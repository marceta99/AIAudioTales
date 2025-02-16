using System.ComponentModel.DataAnnotations;

namespace Kumadio.Core.Models
{
    public class BookModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
        public int CreatorId { get; set; }
    }
}
