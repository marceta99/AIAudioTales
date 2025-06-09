using System.Security.Cryptography;
using System.Text;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Models;

namespace Kumadio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IOnboardingQuestionRepository _onboardingQuestionRepository;
        private readonly IOnboardingDataRepository _onboardingDataRepository;
        private readonly ISelectedOptionRepository _selectedOptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IRefreshTokenRepository refreshTokenRepository,
            IOnboardingQuestionRepository onboardingQuestionRepository,
            IOnboardingDataRepository onboardingDataRepository,
            ISelectedOptionRepository selectedOptionRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _onboardingQuestionRepository = onboardingQuestionRepository;
            _onboardingDataRepository = onboardingDataRepository;
            _selectedOptionRepository = selectedOptionRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        #region Registration & Login
        public async Task<Result> Register(User user, string password)
        {
            if (user == null) return DomainErrors.Auth.UserNull;

            // Hash password
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var userExists = await _userRepository.Any(u => u.Email == user.Email);

            if (userExists)
            {
                return DomainErrors.Auth.EmailAlreadyExists;
            }

            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                await _userRepository.Add(user);

                return Result.Success();
            });
        }

        public async Task<Result> GoogleRegister(User user)
        {
            if (user == null) 
                return DomainErrors.Auth.UserNull;

            var userExists = await _userRepository.Any(u => u.Email == user.Email);

            if (userExists)
                return DomainErrors.Auth.EmailAlreadyExists;
            
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                await _userRepository.Add(user);

                return Result.Success();
            });
        }

        public async Task<Result> RegisterCreator(User user, string password)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var userExists = await _userRepository.Any(u => u.Email == user.Email);

            if (userExists)
            {
                return DomainErrors.Auth.EmailAlreadyExists;
            }

            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                await _userRepository.Add(user);

                return Result.Success();
            });
        }

        public async Task<Result<User>> Login(string email, string password)
        {
            var user = await _userRepository.GetFirstWhere(u => u.Email == email);
            if (user == null)
            {
                return DomainErrors.Auth.UserEmailNotFound;
            }

            var passwordMatch = PasswordMatch(password, user);
            if (!passwordMatch)
            {
                return DomainErrors.Auth.WrongPassword;
            }

            return user;
        }

        public Result<IEnumerable<OnboardingQuestion>> GetOnboardingQuestions()
        {
            var questions = _onboardingQuestionRepository.GetAllQuestions();

            return Result<IEnumerable<OnboardingQuestion>>.Success(questions);
        }

        #endregion

        #region Tokens
        public async Task<Result<RefreshToken>> GetRefreshToken(string refreshTokenHash)
        {
            var token = await _refreshTokenRepository.GetFirstWhere(rt => rt.Token == refreshTokenHash);

            if (token == null) return DomainErrors.Auth.RefreshTokenNotFound;

            return token;
        }

        public async Task<Result<User>> GetUserWithRefreshToken(RefreshToken refreshToken)
        {
            var user = await _userRepository.GetFirstWhere(u => u.Id == refreshToken.UserId);

            if (user == null) return DomainErrors.Auth.UserWithTokenNotFound;

            return user;
        }

        public async Task<Result> DeleteRefreshToken(string refreshTokenHash)
        {
            var refreshToken = await _refreshTokenRepository.GetFirstWhere(rt => rt.Token == refreshTokenHash);

            if (refreshToken == null) return DomainErrors.Auth.RefreshTokenNotFound;

            return await _unitOfWork.ExecuteInTransaction(() =>
            {
                _refreshTokenRepository.Remove(refreshToken);

                // Return a Task so the signature matches `Func<Task<Result>>`
                return Task.FromResult(Result.Success());
            });
        }

        public async Task<Result> DeleteRefreshTokenForUser(string email)
        {
            var user = await _userRepository.GetFirstWhere(u => u.Email == email);

            if (user == null) return DomainErrors.Auth.UserEmailNotFound;

            var refreshToken = await _refreshTokenRepository.GetFirstWhere(rt => rt.UserId == user.Id);

            if (refreshToken == null) return DomainErrors.Auth.RefreshTokenNotFound;

            return await _unitOfWork.ExecuteInTransaction(() =>
            {
                _refreshTokenRepository.Remove(refreshToken);

                // Return a Task so the signature matches `Func<Task<Result>>`
                return Task.FromResult(Result.Success());
            });
        }

        public async Task<Result> SaveRefreshToken(RefreshToken refreshToken, User user)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var existingToken = await _refreshTokenRepository.GetFirstWhere(rt => rt.UserId == user.Id);

                if (existingToken == null)
                {
                    refreshToken.UserId = user.Id;
                    await _refreshTokenRepository.Add(refreshToken);
                }
                else
                {
                    existingToken.Token = refreshToken.Token;
                    existingToken.Expires = refreshToken.Expires;
                    existingToken.Created = refreshToken.Created;
                }

                return Result.Success();
            });
        }

        #endregion

        public async Task<Result<User>> GetUserWithEmail(string email)
        {
            var user = await _userRepository.GetFirstWhere(u => u.Email == email);
            if (user == null)
            {
                return DomainErrors.Auth.UserEmailNotFound;
            }

            return user;
        }
        public async Task<Result> CompleteOnboarding(OnboardingData onboardingData, ICollection<int> selectedOptionsIds)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var user = await _userRepository.GetFirstWhere(u => u.Id == onboardingData.UserId);
                
                user!.IsOnboarded = true;

                var newOnboardingData = await _onboardingDataRepository.AddAndReturn(onboardingData);

                var selectedOptions = new List<SelectedOption>();
                foreach(var optionId in selectedOptionsIds)
                {
                    selectedOptions.Add(new SelectedOption
                    {
                        OnboardingDataId = newOnboardingData.UserId,
                        OnboardingOptionId = optionId
                    });
                }

                await _selectedOptionRepository.AddRange(selectedOptions);

                return Result.Success();
            });
        }

        #region Private Helper Methods
        private bool PasswordMatch(string password, User user)
        {
            using (HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computed.SequenceEqual(user.PasswordHash);
            }
        }
        #endregion

    }

}