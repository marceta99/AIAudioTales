using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBookRepository
    {
        Task<IList<BookReturnDTO>> GetAllBooks();
        Task<IList<BookReturnDTO>> GetBooksFromSpecificUser(int userId);
        Task<IList<BookReturnDTO>> GetBooksForCategory(BookCategory bookCategory);
        Task<BookReturnDTO> GetBook(int id);
        Task<BookReturnDTO> AddNewBook(BookCreationDTO newBook);
        Task<int> UpdateBookDetails(BookUpdateDTO book);
        Task<int> DeleteBook(int bookId);
        public Task<int> UploadImageForBook(int bookId, IFormFile imageFile);
    }
}
