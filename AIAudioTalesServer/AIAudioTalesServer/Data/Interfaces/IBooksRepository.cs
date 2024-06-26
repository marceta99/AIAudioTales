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
        Task<BookPart> GetRootPart(int bookId);
        Task<DTOReturnPart> GetPart(int partId);
        Task<DTOReturnTreePart> GetBookTree(int bookId);
        #endregion

        #region POST
        Task SaveSearchTerm(int userId, string searchTerm);
        Task<BasketItem> AddBasketItem(int userId, int bookId);
        Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems, PurchaseType purchaseType, Language language, string sessionId);
        Task<DTOReturnPart?> AddBookPart(DTOCreatePart part, HttpRequest request);
        Task<Book> AddBook(DTOCreateBook book, int creatorId);
        Task<DTOReturnPart?> AddRootPart(DTOCreateRootPart root, HttpRequest request);
        Task<string> Upload(IFormFile formFile, HttpRequest request);

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
