using System.ComponentModel.DataAnnotations;

namespace AIAudioTalesServer.Models.Auth
{
    public class Register
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public int CountryId { get; set; }
        public int JobId { get; set; }
        public string? ImageURL { get; set; }
    }  
}
