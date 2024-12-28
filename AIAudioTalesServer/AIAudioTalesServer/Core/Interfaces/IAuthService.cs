using AIAudioTalesServer.Web.DTOS.Auth;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Core.Interfaces
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
    }
}
