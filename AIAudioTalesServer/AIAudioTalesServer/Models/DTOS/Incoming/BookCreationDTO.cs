using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Models.DTOS.Incoming
{
    public class BookCreationDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Category BookCategory { get; set; }

    }
}
