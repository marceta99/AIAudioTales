namespace Kumadio.Web.DTOS
{
    public class DTOReturnPart
    {
        public int Id { get; set; }
        public string PartAudioLink { get; set; }
        public bool IsRoot { get; set; } = false;
        public int BookId { get; set; }
        public int ParentAnwserId { get; set; }
        public string ParentAnswerText { get; set; }
        public IList<DTOReturnAnswer?> Answers { get; set; }
    }
}
