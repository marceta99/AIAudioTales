using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS.Auth
{
    public class DTORefreshToken
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
