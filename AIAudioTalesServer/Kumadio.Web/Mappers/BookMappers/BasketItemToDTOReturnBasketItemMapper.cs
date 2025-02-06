using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookMappers
{
    public class BasketItemToDTOReturnBasketItemMapper : DtoMapper<BasketItem, DTOReturnBasketItem>
    {
        private readonly IDtoMapper<Book, DTOReturnBook> _bookMapper;

        public BasketItemToDTOReturnBasketItemMapper(IDtoMapper<Book, DTOReturnBook> bookMapper)
        {
            _bookMapper = bookMapper;
        }
        public override DTOReturnBasketItem MapCore(BasketItem source)
        {
            return new DTOReturnBasketItem
            {
                Id = source.Id,
                BookId = source.BookId,
                ItemPrice = source.ItemPrice,
                Book = _bookMapper.Map(source.Book)
            };
        }
    }
}
