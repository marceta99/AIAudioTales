using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Core.Interfaces
{
    public interface ILibraryService
    {
        // GET
        Task<bool> UserHasBookAsync(int bookId, int userId);
        Task<bool> IsBasketItemAsync(int bookId, int userId);
        Task<IList<DTOReturnPurchasedBook>> GetPurchasedBooksAsync(int userId);
        Task<IList<DTOReturnBook>> GetCreatorBooksAsync(int userId);
        Task<DTOReturnPurchasedBook?> GetPurchasedBookAsync(int userId, int bookId);
        Task<IList<string>> GetSearchHistoryAsync(int userId);
        Task<DTOBasket> GetBasketAsync(int userId);
        Task<DTOReturnPurchasedBook?> GetCurrentBookAsync(int userId);

        // POST
        Task<DTOBasket> AddBasketItemAsync(int userId, int bookId);
        Task SaveSearchTermAsync(int userId, string searchTerm);

        // PATCH
        Task<bool> AddToLibraryAsync(User user, int bookId);
        Task<bool> UpdatePurchaseStatusAsync(string sessionId);
        Task<DTOReturnPurchasedBook?> NextPartAsync(DTOUpdateNextPart dto, int userId);
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
