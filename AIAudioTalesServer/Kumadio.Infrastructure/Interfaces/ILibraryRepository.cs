using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Infrastructure.Interfaces
{
    public interface ILibraryRepository
    {
        Task<bool> UserHasBookAsync(int bookId, int userId);
        Task<bool> IsBasketItemAsync(int bookId, int userId);
        Task<IList<PurchasedBooks>> GetPurchasedBooksAsync(int userId);
        Task<PurchasedBooks?> GetCurrentPurchasedBookAsync(int userId);

        Task<PurchasedBooks?> GetPurchasedBookAsync(int userId, int bookId);
        Task<IList<string>> GetSearchHistoryAsync(int userId);
        Task<IList<BasketItem>> GetBasketItemsAsync(int userId);
        Task<IList<Book>> GetCreatorBooksAsync(int userId);

        // POST
        Task AddNewSearchTermAsync(int userId, string searchTerm);
        Task<BasketItem?> AddBasketItemAsync(int userId, int bookId);
        Task AddPurchasedBooksAsync(IList<PurchasedBooks> purchases);


        // PATCH
        Task<bool> AddToLibraryAsync(User user, int bookId);
        Task<PurchasedBooks?> GetPurchaseBySessionIdAsync(string sessionId);
        Task UpdatePurchaseAsync(PurchasedBooks purchase);
        Task UpdatePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases);

        // DELETE
        Task RemoveBasketItemAsync(BasketItem item);
        Task RemoveBasketItemsAsync(int userId);
        Task RemovePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases);

        // Additional convenience queries
        Task<IList<PurchasedBooks>> GetPendingPurchasesAsync(User user);

        Task<bool> RemoveUserPendingPurchases(User user);
        Task PurchaseBooks(int userId, IList<int> bookIds, PurchaseType purchaseType, Language language, string sessionId);
        Task RemoveBasketItems(int userId);
        Task<bool> UpdatePurchaseStatus(string sessionId);
        Task<bool> RemoveCanceledPurchase(string sessionId);
        Task<BasketItem?> GetBasketItemByIdAsync(int itemId);
    }
}
