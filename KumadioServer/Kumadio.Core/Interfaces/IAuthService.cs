using Kumadio.Core.Common;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IAuthService
    {
        #region Registration & Login
        Task<Result> Register(User user, string password);
        Task<Result> RegisterCreator(User user, string password);
        Task<Result<User>> Login(string email, string password);
        Result<IEnumerable<OnboardingQuestion>> GetOnboardingQuestions();

        #endregion

        #region Tokens
        Task<Result<RefreshToken>> GetRefreshToken(string refreshTokenHash);
        Task<Result<User>> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task<Result> DeleteRefreshTokenForUser(string email);
        Task<Result> DeleteRefreshToken(string refreshTokenHash);
        Task<Result> SaveRefreshToken(RefreshToken refreshToken, User user);

        #endregion
        Task<Result<User>> GetUserWithEmail(string email);
        Task<Result> CompleteOnboarding(OnboardingData onboardingData);
    }
}
