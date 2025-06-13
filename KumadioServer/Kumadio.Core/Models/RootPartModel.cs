using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class RootPartModel
    {
        public int BookId { get; set; }
        public byte[] AudioBytes { get; set; }
        public string Extension { get; set; }
        public string? QuestionText { get; set; }
        public IList<string> AnswersText { get; set; }
    }
}
