using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.PurchasedBookMappers
{
    public class PurchasedBookToDTOReturnPurchasedBookMapper : DtoMapper<PurchasedBook, DTOReturnPurchasedBook>
    {
        private readonly IDtoMapper<BookPart, DTOReturnPart> _partMapper;

        public PurchasedBookToDTOReturnPurchasedBookMapper(IDtoMapper<BookPart, DTOReturnPart> partMapper)
        {
            _partMapper = partMapper;
        }
        public override DTOReturnPurchasedBook MapCore(PurchasedBook source)
        {
            return new DTOReturnPurchasedBook
            {
                BookId = source.BookId,
                Description = source.Book.Description,
                Title = source.Book.Title,
                ImageURL = source.Book.ImageURL,
                PurchaseType = source.PurchaseType,
                Language = source.Language,
                PlayingPart = _partMapper.Map(source.PlayingPart),
                PlayingPosition = source.PlayingPosition,
                IsBookPlaying = source.IsBookPlaying,
                QuestionsActive = source.QuestionsActive
            };
        }
    }
}
