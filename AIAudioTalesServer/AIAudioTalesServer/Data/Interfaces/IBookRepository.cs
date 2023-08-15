using AIAudioTalesServer.Models;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBookRepository
    {
        Task<IList<Book>> GetAllBooks();
        Task<IList<Book>> GetBooksFromSpecificUser(int userId);
        Task<IList<Book>> GetBooksForCategory(BookCategory bookCategory);
        Task<Book> GetBook(int id);
        Task AddNewBook(Book book);
        Task UpdateBookDetails(Book book);
        Task DeleteBook(int bookId);

    }
}
