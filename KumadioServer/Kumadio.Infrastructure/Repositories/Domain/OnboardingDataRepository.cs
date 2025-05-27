using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class OnboardingDataRepository : Repository<OnboardingData>, IOnboardingDataRepository
    {
        public OnboardingDataRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
