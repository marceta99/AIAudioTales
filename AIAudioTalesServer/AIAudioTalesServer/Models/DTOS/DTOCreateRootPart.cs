namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOCreateRootPart
    {
        public int BookId { get; set; }
        public IFormFile PartAudio { get; set; }
        public IList<DTOCreateAnswer> Answers { get; set; }
    }
}
