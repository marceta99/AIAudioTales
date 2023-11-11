using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBooksRepository
    {
        Task<Book> AddNewBook(BookCreateDTO newBook);
        Task<IList<Book>> GetAllBooks();
        Task<Book> GetBook(int id);
    }
}
