namespace Kumadio.Web.DTOS
{
    public class DTOReturnPartTree
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public IList<DTOReturnAnswer> Answers { get; set; }
        public IList<DTOReturnPartTree> NextParts { get; set; }
    }
}
