namespace AIAudioTalesServer.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public IList<PurchasedBooks> PurchasedBooks { get; set; }
        public IList<BasketItem> BasketItems { get; set; }
        public IList<BookPart> BookParts { get; set; }
    }
}
