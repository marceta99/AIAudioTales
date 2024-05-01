namespace AIAudioTalesServer.Models
{
    public class BookPart
    {
        public int Id { get; set; }
        public string PartAudioLink { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public int ParentAnswerId { get; set; }
        public Answer ParentAnswer { get; set; }
        public IList<Answer> Answers { get; set; }
    }
}
