using Kumadio.Domain.Enums;

namespace Kumadio.Web.DTOS
{
    public class DTOPurchase
    {
        public int BookId { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }

}
