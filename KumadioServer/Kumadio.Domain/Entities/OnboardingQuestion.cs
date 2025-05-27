using Kumadio.Domain.Enums;

namespace Kumadio.Domain.Entities
{
    public class OnboardingQuestion
    {
        public int Id { get; set; }
        public string Key { get; set; }            // npr. "childAge"
        public string Text { get; set; }           // tekst pitanja
        public QuestionType Type { get; set; }     // NumberInput, SingleChoice, MultiChoice
        public ICollection<OnboardingOption> Options { get; set; } = new List<OnboardingOption>();     // samo za choice tipove
        public int Order { get; set; }             // redosled
    }
}
