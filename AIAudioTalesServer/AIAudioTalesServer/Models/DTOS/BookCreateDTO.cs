using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS
{
    public class BookCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public BookCategory BookCategory { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
    }
}
