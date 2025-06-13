using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class PartModel
    {
        public int BookId { get; set; }
        public string? QuestionText { get; set; }
        public int ParentAnswerId { get; set; }
        public byte[] AudioBytes { get; set; }
        public string Extension { get; set; }
        public IList<string> AnswersText { get; set; }
    }
}
