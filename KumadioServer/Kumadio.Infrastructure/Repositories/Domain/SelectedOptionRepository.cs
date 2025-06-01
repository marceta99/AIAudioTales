using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class SelectedOptionRepository : Repository<SelectedOption>, ISelectedOptionRepository
    {
        public SelectedOptionRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
