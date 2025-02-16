using System.ComponentModel.DataAnnotations;

namespace Kumadio.Web.DTOS
{
    public class DTOCreateBook
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
