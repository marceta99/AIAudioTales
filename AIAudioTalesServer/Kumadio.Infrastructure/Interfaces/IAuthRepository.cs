using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Infrastructure.Interfaces
{
    public interface IAuthRepository
    {
        Task<int> AddNewUser(User user);
        Task<bool> IsEmailUsed(string email);
        Task<User?> GetUserWithEmail(string email);
        Task<RefreshToken?> GetRefreshToken(string refreshToken);
        Task<User?> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task SaveRefreshTokenInDb(RefreshToken refreshToken, User user);
        Task DeleteRefreshTokenForUser(string email);
        Task UpdateUserRole(Role role, int userId);
    }
}
