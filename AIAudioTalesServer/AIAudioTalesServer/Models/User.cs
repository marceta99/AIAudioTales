using AIAudioTalesServer.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIAudioTalesServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Role Role { get; set; }
        public IList<PurchasedBooks> PurchasedBooks { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
