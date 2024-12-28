// FILE: BooksRepository.cs
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Data;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Infrastructure.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly AppDbContext _dbContext;

        public BooksRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET
        public async Task<IList<Book>> GetAllBooksAsync()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            return await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<IList<Book>> GetBooksByCategoryAsync(int categoryId, int skip, int take)
        {
            return await _dbContext.Books
                .Where(b => b.CategoryId == categoryId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> UserHasBookAsync(int bookId, int userId)
        {
            var purchase = await _dbContext.PurchasedBooks
                .FirstOrDefaultAsync(pb => pb.BookId == bookId
                                        && pb.UserId == userId
                                        && pb.PurchaseStatus == PurchaseStatus.Success);
            return (purchase != null);
        }

        public async Task<bool> IsBasketItemAsync(int bookId, int userId)
        {
            var basketItem = await _dbContext.BasketItems
                .FirstOrDefaultAsync(bi => bi.UserId == userId && bi.BookId == bookId);
            return (basketItem != null);
        }

        public async Task<IList<PurchasedBooks>> GetPurchasedBooksAsync(int userId)
        {
            return await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb => pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .ToListAsync();
        }

        public async Task<PurchasedBooks?> GetCurrentPurchasedBookAsync(int userId)
        {
            return await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId
                          && pb.IsBookPlaying == true
                          && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb => pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<Book>> GetCreatorBooksAsync(int userId)
        {
            return await _dbContext.Books
                .Where(b => b.CreatorId == userId)
                .ToListAsync();
        }

        public async Task<PurchasedBooks?> GetPurchasedBookAsync(int userId, int bookId)
        {
            return await _dbContext.PurchasedBooks
                .FirstOrDefaultAsync(pb => pb.UserId == userId
                                        && pb.BookId == bookId
                                        && pb.PurchaseStatus == PurchaseStatus.Success);
        }

        public async Task<IList<Book>> SearchBooksAsync(string searchTerm, int skip, int take)
        {
            return await _dbContext.Books
                .Where(b => b.Title.Contains(searchTerm))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<string>> GetSearchHistoryAsync(int userId)
        {
            return await _dbContext.SearchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SearchDate)
                .Take(10)
                .Select(h => h.SearchTerm)
                .ToListAsync();
        }

        public async Task<IList<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.BookCategories.ToListAsync();
        }

        public async Task<IList<BasketItem>> GetBasketItemsAsync(int userId)
        {
            return await _dbContext.BasketItems
                .Where(bi => bi.UserId == userId)
                .Include(bi => bi.Book)
                .ToListAsync();
        }

        public async Task<BasketItem?> GetBasketItemByIdAsync(int itemId)
        {
            return await _dbContext.BasketItems.FindAsync(itemId);
        }

        public async Task<BookPart?> GetBookPartAsync(int partId)
        {
            return await _dbContext.BookParts
                .Include(bp => bp.Answers)
                .Include(bp => bp.ParentAnswer)
                .FirstOrDefaultAsync(bp => bp.Id == partId);
        }

        public async Task<IList<Answer>> GetAnswersForPartAsync(int partId)
        {
            return await _dbContext.Answers
                .Where(a => a.CurrentPartId == partId)
                .ToListAsync();
        }

        public async Task<BookPart?> GetRootPartAsync(int bookId)
        {
            return await _dbContext.BookParts
                .Where(bp => bp.BookId == bookId && bp.IsRoot == true)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }

        // POST
        public async Task AddNewSearchTermAsync(int userId, string searchTerm)
        {
            var newSearch = new SearchHistory
            {
                UserId = userId,
                SearchTerm = searchTerm,
                SearchDate = DateTime.UtcNow
            };
            await _dbContext.SearchHistories.AddAsync(newSearch);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BasketItem?> AddBasketItemAsync(int userId, int bookId)
        {
            var book = await _dbContext.Books.FindAsync(bookId);
            if (book == null) return null;

            var basketItem = new BasketItem
            {
                UserId = userId,
                BookId = bookId,
                ItemPrice = book.Price
            };
            await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();
            return basketItem;
        }

        public async Task AddPurchasedBooksAsync(IList<PurchasedBooks> purchases)
        {
            await _dbContext.PurchasedBooks.AddRangeAsync(purchases);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<BookPart> AddBookPartAsync(BookPart bookPart)
        {
            await _dbContext.BookParts.AddAsync(bookPart);
            await _dbContext.SaveChangesAsync();
            return bookPart;
        }

        public async Task<IList<Answer>> AddAnswersAsync(IList<Answer> answers)
        {
            await _dbContext.Answers.AddRangeAsync(answers);
            await _dbContext.SaveChangesAsync();
            return answers;
        }

        // PATCH
        public async Task<bool> AddToLibraryAsync(User user, Book book)
        {
            // Basic check
            if (user.Role != Role.LISTENER_WITH_SUBSCRIPTION) return false;
            if (book == null) return false;

            var rootPart = await GetRootPartAsync(book.Id);
            if (rootPart == null) return false;

            var pb = new PurchasedBooks
            {
                BookId = book.Id,
                UserId = user.Id,
                PurchaseType = PurchaseType.Enroled,
                Language = Language.ENGLISH_UK,
                PurchaseStatus = PurchaseStatus.Success,
                SessionId = "",
                PlayingPartId = rootPart.Id,
                PlayingPosition = 0,
                IsBookPlaying = false,
                QuestionsActive = false
            };

            await _dbContext.PurchasedBooks.AddAsync(pb);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PurchasedBooks?> GetPurchaseBySessionIdAsync(string sessionId)
        {
            return await _dbContext.PurchasedBooks
                .FirstOrDefaultAsync(pb => pb.SessionId == sessionId);
        }

        public async Task UpdatePurchaseAsync(PurchasedBooks purchase)
        {
            _dbContext.PurchasedBooks.Update(purchase);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases)
        {
            _dbContext.PurchasedBooks.UpdateRange(purchases);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            _dbContext.Answers.Update(answer);
            await _dbContext.SaveChangesAsync();
        }


        // DELETE
        public async Task RemoveBasketItemAsync(BasketItem item)
        {
            _dbContext.BasketItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveBasketItemsAsync(int userId)
        {
            var items = await _dbContext.BasketItems
                .Where(b => b.UserId == userId)
                .ToListAsync();
            _dbContext.BasketItems.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemovePurchasedBooksAsync(IEnumerable<PurchasedBooks> purchases)
        {
            _dbContext.PurchasedBooks.RemoveRange(purchases);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<PurchasedBooks>> GetPendingPurchasesAsync(User user)
        {
            return await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == user.Id && pb.PurchaseStatus == PurchaseStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> RemoveUserPendingPurchases(User user)
        {
            try
            {
                var purchases = await _dbContext.PurchasedBooks
                    .Where(pb => pb.UserId == user.Id && pb.PurchaseStatus == PurchaseStatus.Pending)
                    .ToListAsync();

                if (purchases != null && purchases.Any())
                {
                    _dbContext.PurchasedBooks.RemoveRange(purchases);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false; // No purchases found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove user pending purchases: {ex.Message}");
                return false;
            }
        }

        public async Task PurchaseBooks(
            int userId,
            IList<DTOReturnBasketItem> basketItems,
            PurchaseType purchaseType,
            Language language,
            string sessionId)
        {
            var purchasedBooks = new List<PurchasedBooks>();

            foreach (var basketItem in basketItems)
            {
                var pb = new PurchasedBooks
                {
                    BookId = basketItem.BookId,
                    UserId = userId,
                    PurchaseType = purchaseType,
                    Language = language,
                    PurchaseStatus = PurchaseStatus.Pending,
                    SessionId = sessionId,
                    PlayingPartId = await GetRootPart(basketItem.BookId),
                    PlayingPosition = 0,
                    IsBookPlaying = false,
                    QuestionsActive = false
                };
                purchasedBooks.Add(pb);
            }

            await _dbContext.PurchasedBooks.AddRangeAsync(purchasedBooks);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<int> GetRootPart(int bookId)
        {
            // Example of a method to find the "root part" of a book
            // If you already had logic for this, replicate it here. 
            // For now, assume the root part is the first part with the given BookId
            var part = await _dbContext.BookParts
                .Where(bp => bp.BookId == bookId)
                .OrderBy(bp => bp.Id)
                .FirstOrDefaultAsync();

            return part?.Id ?? 0;
        }

        public async Task RemoveBasketItems(int userId)
        {
            var basketItems = await _dbContext.BasketItems
                .Where(b => b.UserId == userId)
                .ToListAsync();

            _dbContext.BasketItems.RemoveRange(basketItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdatePurchaseStatus(string sessionId)
        {
            try
            {
                var purchase = await _dbContext.PurchasedBooks
                    .Where(pb => pb.SessionId == sessionId)
                    .FirstOrDefaultAsync();

                if (purchase != null)
                {
                    purchase.PurchaseStatus = PurchaseStatus.Success;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false; // No purchase found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to update purchase status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveCanceledPurchase(string sessionId)
        {
            try
            {
                var purchases = await _dbContext.PurchasedBooks
                    .Where(pb => pb.SessionId == sessionId)
                    .ToListAsync();

                if (purchases != null && purchases.Any())
                {
                    _dbContext.PurchasedBooks.RemoveRange(purchases);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false; // No purchase found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove canceled purchase: {ex.Message}");
                return false;
            }
        }
    }
}
