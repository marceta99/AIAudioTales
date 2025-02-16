using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUpdateNextPart
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public int NextPartId { get; set; }
    }
}
