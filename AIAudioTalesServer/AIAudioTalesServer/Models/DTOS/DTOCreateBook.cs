namespace AIAudioTalesServer.Models.DTOS
{
    public class DTOCreateBook
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
        //public IList<DTOCreatePart> BookParts { get; set; }
        public DTOCreateRootPart RootPart { get; set; }
    }
}
