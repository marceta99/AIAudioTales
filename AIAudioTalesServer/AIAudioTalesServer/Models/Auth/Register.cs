using System.ComponentModel.DataAnnotations;

namespace AIAudioTalesServer.Models.Auth
{
    public class Register
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }  
}
