using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class OnboardingQuestionRepository : Repository<OnboardingQuestion>, IOnboardingQuestionRepository
    {
        public OnboardingQuestionRepository(AppDbContext context) : base(context)
        {

        }

        public IEnumerable<OnboardingQuestion> GetAllQuestions()
        {
            return _dbSet.Include(oq => oq.Options);
        }
    }
}
