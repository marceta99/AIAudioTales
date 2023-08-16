namespace AIAudioTalesServer.Models.DTOS.Outgoing
{
    public class BookReturnDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public BookCategory BookCategory { get; set; }
    }
}
