using Kumadio.Infrastructure.Repositories.Base;
using Kumadio.Domain.Entities;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Infrastructure.Data;


namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
