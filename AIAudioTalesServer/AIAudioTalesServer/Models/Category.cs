namespace AIAudioTalesServer.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public IList<Book> BooksFromCategory { get; set; }

    }
}
