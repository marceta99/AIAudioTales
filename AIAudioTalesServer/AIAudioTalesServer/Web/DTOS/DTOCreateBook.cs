namespace AIAudioTalesServer.Web.DTOS
{
    public class DTOCreateBook
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
    }
}
