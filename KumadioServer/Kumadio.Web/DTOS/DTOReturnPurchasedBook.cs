using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Web.DTOS
{
    public class DTOReturnPurchasedBook
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int BookCategory { get; set; }
        public DTOReturnPart PlayingPart { get; set; }
        public decimal PlayingPosition { get; set; }
        public bool IsBookPlaying { get; set; }
        public bool QuestionsActive { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public Language Language { get; set; }
    }
}
