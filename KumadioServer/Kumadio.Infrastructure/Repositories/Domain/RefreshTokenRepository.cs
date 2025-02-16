using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
