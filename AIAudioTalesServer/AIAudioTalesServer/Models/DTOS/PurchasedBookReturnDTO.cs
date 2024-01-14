using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS
{
    public class PurchasedBookReturnDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public BookCategory BookCategory { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }
}
