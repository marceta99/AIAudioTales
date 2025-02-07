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
        Task<IList<PurchasedBook>> GetPurchasedBooksAsync(int userId);
        Task<PurchasedBook?> GetCurrentPurchasedBookAsync(int userId);

        Task<PurchasedBook?> GetPurchasedBookAsync(int userId, int bookId);
        Task<IList<string>> GetSearchHistoryAsync(int userId);
        Task<IList<BasketItem>> GetBasketItemsAsync(int userId);
        Task<IList<Book>> GetCreatorBooksAsync(int userId);

        // POST
        Task AddNewSearchTermAsync(int userId, string searchTerm);
        Task<BasketItem?> AddBasketItemAsync(int userId, int bookId);
        Task AddPurchasedBooksAsync(IList<PurchasedBook> purchases);


        // PATCH
        Task<bool> AddToLibraryAsync(User user, int bookId);
        Task<PurchasedBook?> GetPurchaseBySessionIdAsync(string sessionId);
        Task UpdatePurchaseAsync(PurchasedBook purchase);
        Task UpdatePurchasedBooksAsync(IEnumerable<PurchasedBook> purchases);

        // DELETE
        Task RemoveBasketItemAsync(BasketItem item);
        Task RemoveBasketItemsAsync(int userId);
        Task RemovePurchasedBooksAsync(IEnumerable<PurchasedBook> purchases);

        // Additional convenience queries
        Task<IList<PurchasedBook>> GetPendingPurchasesAsync(User user);

        Task<bool> RemoveUserPendingPurchases(User user);
        Task PurchaseBooks(int userId, IList<int> bookIds, PurchaseType purchaseType, Language language, string sessionId);
        Task RemoveBasketItems(int userId);
        Task<bool> UpdatePurchaseStatus(string sessionId);
        Task<bool> RemoveCanceledPurchase(string sessionId);
        Task<BasketItem?> GetBasketItemByIdAsync(int itemId);
    }
}
