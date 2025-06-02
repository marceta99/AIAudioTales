using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOUserResponse
    {
        [Required]
        public int PartId { get; set; }

        [Required]
        public string Transcript { get; set; } = string.Empty;

        [Required]
        public List<string> PossibleAnswers { get; set; } = new List<string>();
    }
}

