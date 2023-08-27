using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS.Incoming
{
    public class Purchase
    {
        public int BookId { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }

}
