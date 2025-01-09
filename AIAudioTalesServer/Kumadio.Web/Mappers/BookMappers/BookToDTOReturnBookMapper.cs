using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookMappers
{
    public class BookToDTOReturnBookMapper : BaseMapper<Book, DTOReturnBook>
    {
        public override DTOReturnBook MapCore(Book source)
        {
            return new DTOReturnBook
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                ImageURL = source.ImageURL,
                CategoryId = source.CategoryId
            };
        }
    }
}
