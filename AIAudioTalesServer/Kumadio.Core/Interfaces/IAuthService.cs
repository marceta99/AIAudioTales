
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(Register model);
        Task<int> RegisterCreatorAsync(RegisterCreator model);
        Task<DTOReturnUser?> LoginAsync(Login model);
        Task RefreshTokenAsync();
        Task RevokeTokenAsync();
        Task<DTOReturnUser?> LoginWithGoogleAsync(string googleCredentials);
        Task<object?> GetCurrentUserAsync();
        Task<User?> GetUserWithEmail(string email);
    }
}
