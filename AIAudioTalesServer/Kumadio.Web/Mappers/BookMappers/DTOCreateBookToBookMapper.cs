using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.BookMappers
{
    public class DTOCreateBookToBookMapper : BaseMapper<DTOCreateBook, Book>
    {
        public override Book MapCore(DTOCreateBook source)
        {
            return new Book
            {
                Title = source.Title,
                Description = source.Description,
                Price = source.Price,
                ImageURL = source.ImageURL,
                CategoryId = source.CategoryId,
            };
        }
    }
}
