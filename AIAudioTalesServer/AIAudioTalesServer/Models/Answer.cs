namespace AIAudioTalesServer.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public BookPart CurrentPart { get; set; }
        public int CurrentPartId { get; set; }
        public BookPart NextPart { get; set; }
        public int? NextPartId { get; set; }
    }
}
