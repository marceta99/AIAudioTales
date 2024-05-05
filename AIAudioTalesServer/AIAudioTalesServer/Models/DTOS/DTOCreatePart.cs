namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOCreatePart
    {
        public int BookId { get; set; }
        public int ParentAnswerId { get; set; }
        public string PartAudioLink { get; set; }
        public IList<DTOCreateAnswer> Answers { get; set; }
    }
}
