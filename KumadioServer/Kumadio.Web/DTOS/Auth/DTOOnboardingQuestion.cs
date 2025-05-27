using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Web.DTOS.Auth
{
    public class DTOOnboardingQuestion
    {
        public string Key { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public IEnumerable<DTOOnboardingOption> Options { get; set; } = Array.Empty<DTOOnboardingOption>();
    }
}
