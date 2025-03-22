namespace Kumadio.Web.DTOS
{
    public class DTOReturnPartTree
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IEnumerable<DTOReturnAnswer>? Answers { get; set; }
        public IEnumerable<DTOReturnPartTree>? NextParts { get; set; }
    }
}
