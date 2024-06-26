namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOCreatePart
    {
        public int BookId { get; set; }
        public int ParentAnswerId { get; set; }
        public IFormFile PartAudio { get; set; }
        public IList<DTOCreateAnswer> Answers { get; set; }
    }
}
