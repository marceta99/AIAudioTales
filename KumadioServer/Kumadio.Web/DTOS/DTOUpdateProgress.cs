using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUpdateProgress
    {
        [Required]
        public int BookId { get; set; }
        public decimal? PlayingPosition { get; set; }
        public int? NextBookId { get; set; }
        public bool? QuestionsActive { get; set; }
    }
}
