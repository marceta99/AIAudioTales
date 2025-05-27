namespace Kumadio.Domain.Entities
{
    public class OnboardingData
    {
        public int UserId { get; set; }
        public User User { get; set; }

        // polja dynamic form-e
        public int? ChildAge { get; set; }
        public string ChildGender { get; set; }
        public string SelectedInterestsJson { get; set; }
        public string PreferredDuration { get; set; }
    }
}
