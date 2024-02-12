namespace AIAudioTalesServer.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public decimal ItemPrice { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

    }
}
