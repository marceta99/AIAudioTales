namespace AIAudioTalesServer.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public int CurrentPartId { get; set; }
        public Part CurrentPart { get; set; }
        public int? NextPartId { get; set; }
        public Part NextPart { get; set; }

    }
}
