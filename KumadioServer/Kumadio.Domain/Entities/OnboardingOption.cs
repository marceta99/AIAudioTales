namespace Kumadio.Domain.Entities
{
    public class OnboardingOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public OnboardingQuestion Question { get; set; }
        public string Text { get; set; }  // npr. "M", "F", "U", ili "Avantura"
        public int Order { get; set; }  // redosled unutar pitanja
    }
}
