namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOReturnTreePart
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IList<DTOReturnAnswer>? Answers { get; set; }
        public IList<DTOReturnTreePart>? NextParts { get; set; }
    }
}
