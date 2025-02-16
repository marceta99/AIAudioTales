using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
