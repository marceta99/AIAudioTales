using Kumadio.Domain.Enums;

namespace Kumadio.Web.DTOS.Auth
{
    public class DTOOnboardingData
    {
        public int? ChildAge { get; set; }
        public ICollection<int> SelectedOptions { get; set; } = new List<int>();
    }
}
