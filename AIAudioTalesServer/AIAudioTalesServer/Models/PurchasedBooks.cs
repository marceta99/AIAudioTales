using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models
{
    public class PurchasedBooks
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }
}
