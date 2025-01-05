using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS.Auth
{
    public class DTORegisterCreator
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Required]
        [MatchProperty("Password", ErrorMessage = "Password and Confirm password must match.")]
        public string ConfirmPassword { get; set; }
    }
}
