// FILE: IBooksRepository.cs
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Infrastructure.Interfaces
{
    public interface IBooksRepository
    {
        // GET
        Task<IList<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int bookId);
        Task<IList<Book>> GetBooksByCategoryAsync(int categoryId, int skip, int take);
        Task<bool> UserHasBookAsync(int bookId, int userId);
        Task<bool> IsBasketItemAsync(int bookId, int userId);
        Task<IList<PurchasedBooks>> GetPurchasedBooksAsync(int userId);
        Task<PurchasedBooks?> GetCurrentPurchasedBookAsync(int userId);
        Task<IList<Book>> GetCreatorBooksAsync(int userId);
        Task<PurchasedBooks?> GetPurchasedBookAsync(int userId, int bookId);
        Task<IList<Book>> SearchBooksAsync(string searchTerm, int skip, int take);
        Task<IList<string>> GetSearchHistoryAsync(int userId);
        Task<IList<Category>> GetAllCategoriesAsync();
        Task<IList<BasketItem>> GetBasketItemsAsync(int userId);
        Task<BasketItem?> GetBasketItemByIdAsync(int itemId);
        Task<BookPart?> GetBookPartAsync(int partId);
        Task<IList<Answer>> GetAnswersForPartAsync(int partId);
        Task<BookPart?> GetRootPartAsync(int bookId);

        // POST
        Task AddNewSearchTermAsync(int userId, string searchTerm);
        Task<BasketItem?> AddBasketItemAsync(int userId, int bookId);
        Task AddPurchasedBooksAsync(IList<PurchasedBooks> purchases);
        Task<Book> AddBookAsync(Book book);
        Task<BookPart> AddBookPartAsync(BookPart bookPart);
        Task<IList<Answer>> AddAnswersAsync(IList<Answer> answers);

        // PATCH
        Task<bool> AddToLibraryAsync(User user, Book book);
        Task<PurchasedBooks?> GetPurchaseBySessionIdAsync(string sessionId);
        Task UpdatePurchaseAsync(PurchasedBooks purchase);
        Task UpdatePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases);
        Task UpdateAnswerAsync(Answer answer);

        // DELETE
        Task RemoveBasketItemAsync(BasketItem item);
        Task RemoveBasketItemsAsync(int userId);
        Task RemovePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases);

        // Additional convenience queries
        Task<IList<PurchasedBooks>> GetPendingPurchasesAsync(User user);

        Task<bool> RemoveUserPendingPurchases(User user);
        Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems, PurchaseType purchaseType, Language language, string sessionId);
        Task RemoveBasketItems(int userId);
        Task<bool> UpdatePurchaseStatus(string sessionId);
        Task<bool> RemoveCanceledPurchase(string sessionId);
    }
}
