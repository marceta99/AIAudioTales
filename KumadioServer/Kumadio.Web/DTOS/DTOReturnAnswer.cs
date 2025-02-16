namespace Kumadio.Web.DTOS
{
    public class DTOReturnAnswer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CurrentPartId { get; set; }
        public int? NextPartId { get; set; }
    }
}
