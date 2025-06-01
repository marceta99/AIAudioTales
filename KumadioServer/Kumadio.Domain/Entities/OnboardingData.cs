using Kumadio.Domain.Enums;

namespace Kumadio.Domain.Entities
{
    public class OnboardingData
    {
        public int UserId { get; set; }
        public User User { get; set; }

        // polja dynamic form-e
        public int? ChildAge { get; set; }
        public List<SelectedOption> SelectedOptions { get; set; }
    }
}
