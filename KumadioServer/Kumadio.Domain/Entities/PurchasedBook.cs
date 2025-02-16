using Kumadio.Domain.Enums;

namespace Kumadio.Domain.Entities
{
    public class PurchasedBook
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
    }
}
