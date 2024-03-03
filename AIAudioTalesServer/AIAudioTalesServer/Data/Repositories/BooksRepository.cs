using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Security.Policy;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public BooksRepository(AppDbContext dbContext, IMapper mapper, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<Book> AddNewBook(BookCreateDTO newBook)
        {
            var book = _mapper.Map<Book>(newBook);
            
            var createdBook = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            return createdBook.Entity;
        }
        public async Task<IList<Book>> GetAllBooks()
        {
            var books = await _dbContext.Books.ToListAsync();
            return books;
        }

        public async Task<Book> GetBook(int id)
        {
            var book = await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
            return book;
        }

        public async Task<IList<BookReturnDTO>> GetBooksForCategory(BookCategory bookCategory)
        {
            var books = await _dbContext.Books.Where(b => b.BookCategory == bookCategory).ToListAsync();
            var returnBooks = _mapper.Map<IList<BookReturnDTO>>(books);

            return returnBooks;
        }

        public async Task PurchaseBooks(int userId, IList<BasketItemReturnDTO> basketItems, PurchaseType purchaseType, Language language, string sessionId)
        {
            List<PurchasedBooks> purchasedBooks = new List<PurchasedBooks>();
            foreach (BasketItemReturnDTO basketItem in basketItems)
            {
                PurchasedBooks pb = new PurchasedBooks
                {
                    BookId = basketItem.BookId,
                    UserId = userId,
                    PurchaseType = purchaseType,
                    Language = language,
                    PurchaseStatus = PurchaseStatus.Pending,
                    SessionId = sessionId
                };
                purchasedBooks.Add(pb);
            }
            
            await _dbContext.PurchasedBooks.AddRangeAsync(purchasedBooks);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdatePurchaseStatus(string sessionId)
        {
            try
            {
                var purchase = await _dbContext.PurchasedBooks.Where(pb => pb.SessionId == sessionId).FirstOrDefaultAsync();

                if (purchase != null)
                {
                    purchase.PurchaseStatus = PurchaseStatus.Success;
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate success
                }
                return false; // No purchase found to update
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Indicate failure
            }
        }

        public async Task<bool> RemoveCanceledPurchase(string sessionId)
        {
            try
            {
                // Find the purchase by sessionId
                var purchases = await _dbContext.PurchasedBooks
                                               .Where(pb => pb.SessionId == sessionId)
                                               .ToListAsync();

                if (purchases != null)
                {
                    // Remove the found purchase from the DbContext
                    _dbContext.PurchasedBooks.RemoveRange(purchases);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate that the removal was successful
                }
                else
                {
                    // No purchase found with the given sessionId
                    return false; // Indicate that no purchase was found to remove
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove a purchase: {ex.Message}");
                return false; // Indicate that an error occurred during the removal process
            }
        }

        public async Task<bool> UserHasBook(int bookId, int userId)
        {
            var purchase = await _dbContext.PurchasedBooks.Where(pb => 
                                                                 pb.BookId == bookId &&
                                                                 pb.UserId == userId &&
                                                                 pb.PurchaseStatus == PurchaseStatus.Success)
                                                                .FirstOrDefaultAsync();
            if (purchase == null) return false;
            return true;
        }

        public async Task<bool> IsBasketItem(int bookId, int userId)
        {
            var basketItem = await _dbContext.BasketItems.Where(bi => bi.UserId == userId && bi.BookId == bookId).FirstOrDefaultAsync();
            if (basketItem == null) return false;
            return true;
        }

        public async Task<IList<PurchasedBookReturnDTO>> GetUserBooks(int userId)
        {
            var purchasedBooks = await _dbContext.PurchasedBooks.Where(pb => pb.UserId == userId && pb.PurchaseStatus == PurchaseStatus.Success).ToListAsync();

            List<PurchasedBookReturnDTO> books = new List<PurchasedBookReturnDTO>();

            foreach (var pb in purchasedBooks)
            {
                var book = await GetBook(pb.BookId); //type Book
                var purchasedBook = new PurchasedBookReturnDTO
                {
                    Id = book.Id,
                    Description = book.Description,
                    Title = book.Title,
                    ImageURL = book.ImageURL,
                    PurchaseType = pb.PurchaseType,
                    Language = pb.Language
                };

                books.Add(purchasedBook);
            }

            return books;
        }

        public async Task<PurchasedBookReturnDTO> GetPurchasedBook(int userId, int bookId)
        {
            var pb = await _dbContext.PurchasedBooks.Where(
                                                    pb => pb.UserId == userId &&
                                                    pb.BookId == bookId &&
                                                    pb.PurchaseStatus == PurchaseStatus.Success)
                                                    .FirstOrDefaultAsync();

            if (pb == null) return null;
            
            var book = await GetBook(pb.BookId);

            var purchasedBook = new PurchasedBookReturnDTO 
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                CategoryId = book.CategoryId,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language
            };

            return purchasedBook;
        }

        public async Task<IEnumerable<Book>> SearchBooks(string searchTerm, int pageNumber, int pageSize)
        {
            string cacheKey = $"Search_{searchTerm}_{pageNumber}_{pageSize}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Book> books))
            {
                books = await _dbContext.Books
                                       .Where(b => b.Title.Contains(searchTerm))
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, books, cacheEntryOptions);
            }

            return books;
        }
        public async Task<IEnumerable<string>> GetSearchHistory(int userId)
        {
            var history = await _dbContext.SearchHistories
                                    .Where(h => h.UserId == userId)
                                    .OrderByDescending(h => h.SearchDate)
                                    .Take(10)
                                    .Select(h => h.SearchTerm)
                                    .ToListAsync();

            return history;
        }

        public async Task SaveSearchTerm(int userId, string searchTerm)
        {
            // Check if the same search term already exists for this user
            var existingTerm = await _dbContext.SearchHistories
                                             .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.SearchTerm == searchTerm);

            if (existingTerm == null)
            {
                // Get the count of search terms for this user
                var count = await _dbContext.SearchHistories.CountAsync(sh => sh.UserId == userId);

                // If there are already 10 search terms, remove the oldest one
                if (count >= 10)
                {
                    var oldestSearchTerm = await _dbContext.SearchHistories
                                                         .Where(sh => sh.UserId == userId)
                                                         .OrderBy(sh => sh.SearchDate)
                                                         .FirstOrDefaultAsync();
                    if (oldestSearchTerm != null)
                    {
                        _dbContext.SearchHistories.Remove(oldestSearchTerm);
                    }
                }

                // Add the new search term
                var newSearchHistory = new SearchHistory
                {
                    UserId = userId,
                    SearchTerm = searchTerm,
                    SearchDate = DateTime.UtcNow
                };
                _dbContext.SearchHistories.Add(newSearchHistory);

                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task<IList<Category>> GetAllCategories()
        {
            var categories = await _dbContext.BookCategories.ToListAsync();
            return categories;
        }

        public async Task<BasketDTO> GetBasket(int userId)
        {
            var basketItems = await _dbContext.BasketItems
                                              .Where(bi => bi.UserId == userId)
                                              .Include(bi => bi.Book)
                                              .ToListAsync();
            var itemsDto = _mapper.Map<IList<BasketItemReturnDTO>>(basketItems);
            var totalPrice = 0m;

            foreach (var item in itemsDto)
            {
                totalPrice += item.ItemPrice;
            }

            var basket = new BasketDTO
            {
                BasketItems = itemsDto,
                TotalPrice = totalPrice
            };

            return basket;
        }

        public async Task<BasketItem> AddBasketItem(int userId, int bookId)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return null;

            var basketItem = new BasketItem
            {
                UserId = userId,
                BookId = bookId,
                ItemPrice = book.Price
            };

            var item = await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();

            return item.Entity;
        }

        public async Task<BasketItem?> GetItemById(int itemId)
        {
            return await _dbContext.BasketItems.FindAsync(itemId);            
        }


        public async Task RemoveBasketItem(BasketItem item)
        {
            _dbContext.BasketItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveBasketItems(int userId)
        {
            var basketItems = await _dbContext.BasketItems.Where(b => b.UserId == userId).ToListAsync();

            _dbContext.BasketItems.RemoveRange(basketItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveUserPendingPurchases(User user)
        {
            try
            {
                // Find the purchase by sessionId
                var purchases = await _dbContext.PurchasedBooks
                                               .Where(pb => pb.UserId == user.Id && pb.PurchaseStatus == PurchaseStatus.Pending)
                                               .ToListAsync();

                if (purchases != null)
                {
                    // Remove the found purchase from the DbContext
                    _dbContext.PurchasedBooks.RemoveRange(purchases);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate that the removal was successful
                }
                else
                {
                    // No purchase found with the given sessionId
                    return false; // Indicate that no purchase was found to remove
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove a purchase: {ex.Message}");
                return false; // Indicate that an error occurred during the removal process
            }
        }
    }
}
