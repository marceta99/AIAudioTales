using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class PartTreeModel
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IEnumerable<Answer>? Answers { get; set; }
        public IEnumerable<PartTreeModel>? NextParts { get; set; }
    }
}
