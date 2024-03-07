using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IAuthRepository
    {
        Task<int> AddNewUser(User user);
        Task<bool> IsEmailUsed(string userName);
        bool CheckPassword(string password, User user);
        Task<User> GetUserWithEmail(string email);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task<User> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task SaveRefreshTokenInDb(RefreshToken refreshToken, User user);
        Task DeleteRefreshTokenForUser(string email);
        Task UpdateUserRole(Role role, int userId);
    }
}
