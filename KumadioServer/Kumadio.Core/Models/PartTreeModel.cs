using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class PartTreeModel
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<PartTreeModel> NextParts { get; set; }
    }
}
