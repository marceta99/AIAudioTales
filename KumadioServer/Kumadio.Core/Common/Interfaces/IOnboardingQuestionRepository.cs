using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Common.Interfaces
{
    public interface IOnboardingQuestionRepository : IRepository<OnboardingQuestion>
    {
        public IEnumerable<OnboardingQuestion> GetAllQuestions();
    }
}
