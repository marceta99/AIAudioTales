using Kumadio.Domain.Entities;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;

namespace Kumadio.Web.Mappers.CategoryMappers
{
    public class CategoryToDTOReturnCategoryMapper : BaseMapper<Category, DTOReturnCategory>
    {
        public override DTOReturnCategory MapCore(Category source)
        {
            return new DTOReturnCategory
            {
                Id = source.Id,
                CategoryName = source.CategoryName,
                Description = source.Description
            };  
        }
    }
}
