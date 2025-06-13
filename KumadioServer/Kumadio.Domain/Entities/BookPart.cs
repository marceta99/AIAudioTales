namespace Kumadio.Domain.Entities
{
    public class BookPart
    {
        public int Id { get; set; }
        public string PartAudioLink { get; set; }
        public bool IsRoot { get; set; } = false;
        public string? QuestionText { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public Answer ParentAnswer { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<PurchasedBook> PurchasedBooks { get; set; }

    }
}
