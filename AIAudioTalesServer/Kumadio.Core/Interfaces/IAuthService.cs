using Kumadio.Core.Common;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(User user, string password);
        Task<Result> RegisterCreatorAsync(User user, string password);
        Task<Result<User?>> Login(string email, string password);
        Task<Result<User?>> GetUserWithEmail(string email);
        Task<Result<RefreshToken>> GetRefreshToken(string refreshTokenHash);
        Task<Result<User?>> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task<Result> DeleteRefreshTokenForUser(string email);
        Task SaveRefreshToken(RefreshToken refreshToken, User user);
     
    }
}
