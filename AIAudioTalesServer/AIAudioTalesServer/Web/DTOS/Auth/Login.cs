using System.ComponentModel.DataAnnotations;

namespace AIAudioTalesServer.Web.DTOS.Auth
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
