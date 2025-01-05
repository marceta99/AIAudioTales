using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(User user, string password);
        Task<int> RegisterCreatorAsync(User user, string password);
        Task<User?> ValidateLogin(string email, string password);
        Task<User?> GetUserWithEmail(string email);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task<User> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task DeleteRefreshTokenForUser(string email);
        Task SaveRefreshToken(RefreshToken refreshToken, User user);
     
    }
}
