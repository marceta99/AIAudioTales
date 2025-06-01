namespace Kumadio.Domain.Entities
{
    public class OnboardingOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public OnboardingQuestion Question { get; set; }
        public string Text { get; set; }  // example: "M", "F", "U", or "Adventure, Math, History"
        public int Order { get; set; }  // order inside question
        public ICollection<SelectedOption> SelectedOptions { get; set; } = new List<SelectedOption>();
    }
}
