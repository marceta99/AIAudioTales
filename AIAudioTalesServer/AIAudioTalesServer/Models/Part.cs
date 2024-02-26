namespace AIAudioTalesServer.Models
{
    public class Part
    {
        public int PartId { get; set; }
        public string PartAudioAWSLink { get; set; }
        public string PartText { get; set;}
        public int StoryId { get; set; }
        public Story Story { get; set; }
        public IList<Answer> Answers { get; set; }
        public Answer ParentAnswer { get; set; }

    }
}
