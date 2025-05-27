using Kumadio.Domain.Entities;

namespace Kumadio.Web.DTOS.Auth
{
    public class DTOOnboardingOption
    {
        public int Id { get; set; }
        public string Text { get; set; }  // npr. "M", "F", "U", ili "Avantura"
    }
}
