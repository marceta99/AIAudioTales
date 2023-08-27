using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IAuthRepository
    {
        Task<int> AddNewUser(User user);
        Task<bool> IsUserNameUsed(string userName);
        bool CheckPassword(string password, User user);
        Task<User> GetUserWithUserName(string userName);
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task<User> GetUserWithRefreshToken(RefreshToken refreshToken);
        Task SaveRefreshTokenInDb(RefreshToken refreshToken, User user);
        Task DeleteRefreshTokenForUser(string userName);
    }
}
