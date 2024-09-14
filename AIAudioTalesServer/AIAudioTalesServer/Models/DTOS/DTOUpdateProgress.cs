namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOUpdateProgress
    {
        public int BookId { get; set; }
        public decimal? PlayingPosition { get; set; }
        public int? NextBookId { get; set; }
        public bool? QuestionsActive { get; set; }
    }
}
