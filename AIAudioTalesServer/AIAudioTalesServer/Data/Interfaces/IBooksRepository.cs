using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBooksRepository
    {
        Task<Book> AddNewBook(BookCreateDTO newBook);
        Task<IList<Book>> GetAllBooks();
        Task<Book> GetBook(int id);
        Task<IList<BookReturnDTO>> GetBooksForCategory(BookCategory bookCategory);
    }
}
