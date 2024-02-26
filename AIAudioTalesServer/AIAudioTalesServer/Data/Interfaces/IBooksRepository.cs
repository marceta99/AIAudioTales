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
        Task PurchaseBook(int userId, int bookId, PurchaseType purchaseType, Language language);
        Task<bool> UserHasBook(int bookId, int userId);
        Task<bool> IsBasketItem(int bookId, int userId);
        Task<IList<PurchasedBookReturnDTO>> GetUserBooks(int userId);
        Task<PurchasedBookReturnDTO> GetPurchasedBook(int userId, int bookId);
        Task<IEnumerable<Book>> SearchBooks(string searchTerm, int pageNumber, int pageSize);
        Task <IEnumerable<string>> GetSearchHistory(int id);
        Task SaveSearchTerm(int userId, string searchTerm);
        Task<IList<Category>> GetAllCategories();
        Task<BasketDTO> GetBasket(int userId);
        Task<BasketItem> AddBasketItem(int userId, int bookId);
        Task<BasketItem> GetItemById(int itemId);
        Task RemoveBasketItem(BasketItem item);
        Task PurchaseBooks(int userId, IList<BasketItemReturnDTO> basketItems, PurchaseType purchaseType, Language language);

    }
}
