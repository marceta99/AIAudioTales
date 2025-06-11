using System.Security.Cryptography;
using System.Text;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Models;
using Kumadio.Domain.Enums;

namespace Kumadio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IOnboardingQuestionRepository _onboardingQuestionRepository;
        private readonly IOnboardingDataRepository _onboardingDataRepository;
        private readonly ISelectedOptionRepository _selectedOptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IRefreshTokenRepository refreshTokenRepository,
            IOnboardingQuestionRepository onboardingQuestionRepository,
            IOnboardingDataRepository onboardingDataRepository,
            ISelectedOptionRepository selectedOptionRepository,
            IUserRepository userRepository,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _onboardingQuestionRepository = onboardingQuestionRepository;
            _onboardingDataRepository = onboardingDataRepository;
            _selectedOptionRepository = selectedOptionRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
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
        public async Task<Result> SendConfirmationEmail(string link, User user)
        {
            var html = $@"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                  <meta charset=""UTF-8"">
                  <title>Confirm your email</title>
                </head>
                <body style=""margin:0; padding:0; background:#F7F8FA; font-family: 'Segoe UI', Arial, sans-serif;"">
                  <!-- Preheader Text -->
                  <div style=""display:none; max-height:0; overflow:hidden; color:#F7F8F7; line-height:1px; font-size:1px;"">
                    Confirm your Kumadio account to get started.
                  </div>

                  <table role=""presentation"" width=""100%"" style=""border-collapse:collapse;"">
                    <tr>
                      <td align=""center"" style=""padding:40px 10px;"">
                        <table role=""presentation"" width=""600"" style=""max-width:600px; background:#FFFFFF; border-radius:8px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.05);"">
                          <tr>
                            <td align=""center"" style=""padding:30px 0;"">
                              <img src=""https://media.licdn.com/dms/image/v2/D4E0BAQHrhPezbdG85A/company-logo_200_200/company-logo_200_200/0/1730828228363/kumadio_logo?e=1755129600&v=beta&t=XzIqUJHALyVu_ENRXM7S-EfipJdujjBpqsXgTsV61Fs"" alt=""Kumadio"" width=""120"" style=""height:auto; display:block;""/>
                            </td>
                          </tr>
                          <tr>
                            <td align=""center"" style=""padding:0 40px;"">
                              <h1 style=""margin:0; font-size:24px; color:#212121;"">Dobrodošli u Kumadio!</h1>
                            </td>
                          </tr>
                          <tr>
                            <td align=""center"" style=""padding:20px 40px; color:#555555; font-size:16px; line-height:1.5;"">
                              <p style=""margin:0 0 16px;"">
                                Hvala što ste se prijavili! Samo jedan korak vas deli od interaktivnih audio priča za decu.
                              </p>
                              <p style=""margin:0;"">
                                Potvrdite vaš mejl klikom na dugme ispod:
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td align=""center"" style=""padding:20px 40px;"">
                              <a href=""{link}""
                                 style=""background:#4A90E2; color:#FFFFFF; text-decoration:none; padding:14px 28px; border-radius:4px; font-size:16px; display:inline-block;"">
                                Potvrdi mejl adresu
                              </a>
                            </td>
                          </tr>
                          <tr>
                            <td align=""center"" style=""padding:0 40px 30px; color:#999999; font-size:14px; line-height:1.4;"">
                              <p style=""margin:0;"">
                                Ako dugme ne radi, kopirajte i nalepite ovaj URL u vaš pregledač:
                              </p>
                              <p style=""word-break:break-all; margin:8px 0 0;"">
                                <a href=""{link}"" style=""color:#4A90E2; text-decoration:none;"">
                                  {link}
                                </a>
                              </p>
                            </td>
                          </tr>
                          <tr>
                            <td align=""center"" style=""background:#F0F0F0; padding:20px; color:#999999; font-size:12px;"">
                              &copy; 2025 Kumadio Ltd. Sva prava zadržana.<br/>
                              Belgrade, Serbia
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(user.Email, "Verifikacija Mejla", html);

            return Result.Success();
        }
        public async Task<Result> MarkEmailAsConfirmed(int userId)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var user = await _userRepository.GetById(userId);

                if (user == null)
                    return DomainErrors.Auth.UserEmailNotFound;

                user.IsEmailConfirmed = true;

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

            if (user.AuthProvider != AuthProvider.Local)
                return DomainErrors.Auth.InvalidLoginMethod;

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