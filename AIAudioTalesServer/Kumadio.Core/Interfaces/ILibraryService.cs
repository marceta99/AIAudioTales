using Kumadio.Core.Common;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface ILibraryService
    {
        // GET
        Task<Result<bool>> UserHasBook(int bookId, int userId);
        Task<Result<IList<PurchasedBook>>> GetPurchasedBooks(int userId);
        Task<Result<IList<Book>>> GetCreatorBooks(int userId);
        Task<Result<PurchasedBook>> GetPurchasedBook(int userId, int bookId);
        Task<Result<IList<string>>> GetSearchHistory(int userId);
        Task<Result<PurchasedBook>> GetCurrentBook(int userId);

        // POST
        Task<Result> AddSearchTerm(int userId, string searchTerm);

        // PATCH
        Task<Result> AddToLibrary(User user, int bookId);
        Task<Result<PurchasedBook>> NextPart(int bookId, int nextPartId, int userId);
        Task<DTOReturnPurchasedBook?> ActivateQuestionsAsync(int bookId, int userId, decimal playingPosition);
        Task<DTOReturnPurchasedBook?> UpdateProgressAsync(DTOUpdateProgress dto, int userId);
        Task<DTOReturnPurchasedBook?> StartBookAgainAsync(int bookId, int userId);

        // DELETE
        Task<DTOBasket> RemoveBasketItemAsync(int userId, int itemId);
        Task<bool> RemoveUserPendingPurchases(User user);
        Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems, PurchaseType purchaseType, Language language, string sessionId);
        Task RemoveBasketItems(int userId);
        Task<bool> UpdatePurchaseStatus(string sessionId);
        Task<bool> RemoveCanceledPurchase(string sessionId);
    }
}
