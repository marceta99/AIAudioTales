using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOReturnPurchasedBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public Category BookCategory { get; set; }
        public int CategoryId { get; set; }
        public DTOReturnPart PlayingPart { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }
}
