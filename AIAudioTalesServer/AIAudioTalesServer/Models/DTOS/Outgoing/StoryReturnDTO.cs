namespace AIAudioTalesServer.Models.DTOS.Outgoing
{
    public class StoryReturnDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public int BookId { get; set; }
        public string AudioDataUrl { get; set; }

    }
}
