using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUserResponse
    {
        [Required]
        public string Prompt { get; set; }
    }
}
