using System.Security.Cryptography;
using System.Text;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;

namespace Kumadio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IBookRepository bookRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        #region Registration & Login
        public async Task<Result> RegisterAsync(User user, string password)
        {
            if (user == null) return DomainErrors.Auth.UserNull;

            // Hash password
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var userExists = await _userRepository.AnyAsync(u => u.Email == user.Email);

            if (!userExists)
            {
                return DomainErrors.Auth.EmailAlreadyExists;
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userRepository.AddAsync(user);

                return Result.Success();
            });
        }

        public async Task<Result> RegisterCreatorAsync(User user, string password)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var userExists = await _userRepository.AnyAsync(u => u.Email == user.Email);

            if (userExists)
            {
                return DomainErrors.Auth.EmailAlreadyExists;
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userRepository.AddAsync(user);

                return Result.Success();
            });
        }

        public async Task<Result<User>> Login(string email, string password)
        {
            var user = await _userRepository.GetFirstWhereAsync(u => u.Email == email);
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

        #endregion

        #region Tokens
        public async Task<Result<RefreshToken>> GetRefreshToken(string refreshTokenHash)
        {
            var token = await _refreshTokenRepository.GetFirstWhereAsync(rt => rt.Token == refreshTokenHash);

            if (token == null) return DomainErrors.Auth.RefreshTokenNotFound;

            return token;
        }

        public async Task<Result<User>> GetUserWithRefreshToken(RefreshToken refreshToken)
        {
            var user = await _userRepository.GetFirstWhereAsync(u => u.Id == refreshToken.UserId);

            if (user == null) return DomainErrors.Auth.UserWithTokenNotFound;

            return user;
        }

        public async Task<Result> DeleteRefreshTokenForUser(string email)
        {
            var user = await _userRepository.GetFirstWhereAsync(u => u.Email == email);

            if (user == null) return DomainErrors.Auth.UserEmailNotFound;

            var refreshToken = await _refreshTokenRepository.GetFirstWhereAsync(rt => rt.UserId == user.Id);

            if (refreshToken == null) return DomainErrors.Auth.RefreshTokenNotFound;

            return await _unitOfWork.ExecuteInTransactionAsync(() =>
            {
                _refreshTokenRepository.Remove(refreshToken);

                // Return a Task so the signature matches `Func<Task<Result>>`
                return Task.FromResult(Result.Success());
            });
        }

        public async Task<Result> SaveRefreshToken(RefreshToken refreshToken, User user)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingToken = await _refreshTokenRepository.GetFirstWhereAsync(rt => rt.UserId == user.Id);

                if (existingToken == null)
                {
                    refreshToken.UserId = user.Id;
                    await _refreshTokenRepository.AddAsync(refreshToken);
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
            var user = await _userRepository.GetFirstWhereAsync(u => u.Email == email);
            if (user == null)
            {
                return DomainErrors.Auth.UserEmailNotFound;
            }

            return user;
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