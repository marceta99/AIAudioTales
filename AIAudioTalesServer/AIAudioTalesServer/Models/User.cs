using System.ComponentModel.DataAnnotations;

namespace AIAudioTalesServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? Phone { get; set; }
        public string? ImageURL { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Role Role { get; set; }
        public RefreshToken RefreshToken { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}
