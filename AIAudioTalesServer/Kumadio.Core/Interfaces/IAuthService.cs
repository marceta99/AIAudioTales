
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(User user, string password);
        Task<int> RegisterCreatorAsync(User user, string password);
        Task<DTOReturnUser?> LoginAsync(Login model);
        Task RefreshTokenAsync();
        Task RevokeTokenAsync();
        Task<DTOReturnUser?> LoginWithGoogleAsync(string googleCredentials);
        Task<object?> GetCurrentUserAsync();
        Task<User?> GetUserWithEmail(string email);
    }
}
