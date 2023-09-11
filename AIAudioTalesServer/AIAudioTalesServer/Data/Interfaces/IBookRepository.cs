using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBookRepository
    {
        Task<IList<BookReturnDTO>> GetAllBooks();
        Task<IList<PurchasedBookReturnDTO>> GetBooksFromSpecificUser(int userId);
        Task<IList<BookReturnDTO>> GetBooksForCategory(BookCategory bookCategory);
        Task<BookReturnDTO> GetBook(int id);
        Task<BookReturnDTO> AddNewBook(BookCreationDTO newBook);
        Task<int> UpdateBookDetails(BookUpdateDTO book);
        Task<int> DeleteBook(int bookId);
        Task<int> UploadImageForBook(int bookId, IFormFile imageFile);
        Task<byte[]?> GetBookImage(int bookId);
        Task<IList<Book>> GetAllBooksWithImages();
        Task<Book> GetBookWithImage(int bookId);
        Task PurchaseBook(int userId, int bookId, PurchaseType purchaseType, Language language); 

        

    }
}
