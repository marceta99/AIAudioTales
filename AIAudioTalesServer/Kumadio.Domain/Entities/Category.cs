namespace Kumadio.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public ICollection<Book> BooksFromCategory { get; set; }

    }
}
