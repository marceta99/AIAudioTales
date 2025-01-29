using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class PartTree
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IList<Answer> Answers { get; set; }
        public IList<PartTree> NextParts { get; set; }
    }
}
