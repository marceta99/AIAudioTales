namespace Kumadio.Web.DTOS.Auth
{
    public class DTOOnboardingData
    {
        public int? ChildAge { get; set; }
        public string ChildGender { get; set; }
        public string[] SelectedInterests { get; set; }
        public string PreferredDuration { get; set; }
    }
}
