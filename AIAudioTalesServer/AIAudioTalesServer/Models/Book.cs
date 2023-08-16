namespace AIAudioTalesServer.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public BookCategory BookCategory { get; set; }
        public byte[]? ImageData { get; set; }
        public IList<PurchasedBooks> PurchasedBooks { get; set; }
    }
}
