namespace AIAudioTalesServer.Models.DTOS
{
    public class BasketItemReturnDTO
    {
        public int Id { get; set; }
        public decimal ItemPrice { get; set; }
        public int BookId { get; set; }
        public BookReturnDTO Book { get; set; }
    }
}
