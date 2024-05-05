namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOCreateRootPart
    {
        public string PartAudioLink { get; set; }
        public IList<DTOCreateAnswer> Answers { get; set; }
    }
}
