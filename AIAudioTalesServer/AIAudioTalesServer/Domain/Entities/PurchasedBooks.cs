using AIAudioTalesServer.Domain.Enums;

namespace AIAudioTalesServer.Domain.Entities
{
    public class PurchasedBooks
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int PlayingPartId { get; set; }
        public BookPart PlayingPart { get; set; }
        public decimal PlayingPosition { get; set; }
        public bool IsBookPlaying { get; set; } // this boolean represent if user is currently playing this book or not
        public bool QuestionsActive { get; set; } // this boolean represent if user is on part of the book where he need to select question answer
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
        public PurchaseStatus PurchaseStatus { get; set; }
        public string SessionId { get; set; }
    }
}
