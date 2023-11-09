using System.Diagnostics;

namespace AIAudioTalesServer.Models
{
    public class Story
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public string Text { get; set; }
        //public byte[]? AudioData { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
        public IList<Part> Parts { get; set; }
    }
}
