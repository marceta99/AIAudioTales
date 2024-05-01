using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IBooksRepository
    {
        #region GET
        Task<IList<Book>> GetAllBooks();
        Task<Book> GetBook(int id);
        Task<IList<DTOReturnBook>> GetBooksForCategory(int bookCategory);
        Task<bool> UserHasBook(int bookId, int userId);
        Task<bool> IsBasketItem(int bookId, int userId);
        Task<IList<DTOReturnPurchasedBook>> GetUserBooks(int userId);
        Task<DTOReturnPurchasedBook> GetPurchasedBook(int userId, int bookId);
        Task<IEnumerable<Book>> SearchBooks(string searchTerm, int pageNumber, int pageSize);
        Task <IEnumerable<string>> GetSearchHistory(int id);
        Task<IList<Category>> GetAllCategories();
        Task<DTOBasket> GetBasket(int userId);
        Task<BasketItem> GetItemById(int itemId);
        #endregion

        #region POST
        Task SaveSearchTerm(int userId, string searchTerm);
        Task<BasketItem> AddBasketItem(int userId, int bookId);
        Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems, PurchaseType purchaseType, Language language, string sessionId);
        #endregion

        #region DELETE
        Task RemoveBasketItem(BasketItem item);
        Task<bool> UpdatePurchaseStatus(string sessionId);
        Task<bool> RemoveCanceledPurchase(string sessionId);
        Task RemoveBasketItems(int userId);
        Task<bool> RemoveUserPendingPurchases(User user);
        #endregion
    }
}
