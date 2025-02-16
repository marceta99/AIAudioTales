using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUpdateProgress
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public decimal? PlayingPosition { get; set; }
        
        [Required]
        public int? NextBookId { get; set; }
        
        [Required]
        public bool? QuestionsActive { get; set; }
    }
}
