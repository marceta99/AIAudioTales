namespace Kumadio.Domain.Entities
{
    public class SelectedOption
    {
        public int OnboardingDataId { get; set; }
        public OnboardingData OnboardingData { get; set; }
        public int OnboardingOptionId { get; set; }
        public OnboardingOption OnboardingOption { get; set; }
    }
}
