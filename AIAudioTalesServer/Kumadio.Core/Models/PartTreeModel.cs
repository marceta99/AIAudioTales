using Kumadio.Domain.Entities;

namespace Kumadio.Core.Models
{
    public class PartTreeModel
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IList<Answer> Answers { get; set; }
        public IList<PartTreeModel> NextParts { get; set; }
    }
}
