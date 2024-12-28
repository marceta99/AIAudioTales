using AIAudioTalesServer.Domain.Enums;

namespace AIAudioTalesServer.Web.DTOS
{
    public class DTOPurchase
    {
        public int BookId { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }

}
