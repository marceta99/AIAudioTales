using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUpdateActivateQuestions
    {
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public decimal PlayingPosition { get; set; }
    }
}
