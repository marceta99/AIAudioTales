using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Infrastructure.Interfaces
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
