using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.Enums;
using Microsoft.Extensions.Caching.Memory;

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

        public async Task PurchaseBook(int userId, int bookId, PurchaseType purchaseType, Language language)
        {
            PurchasedBooks pb = new PurchasedBooks
            {
                BookId = bookId,
                UserId = userId,
                PurchaseType = purchaseType,
                Language = language
            };
            await _dbContext.PurchasedBooks.AddAsync(pb);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UserHasBook(int bookId, int userId)
        {
            var purchase = await _dbContext.PurchasedBooks.Where(pb => pb.BookId == bookId && pb.UserId == userId).FirstOrDefaultAsync();
            if (purchase == null) return false;
            return true;
        }

        public async Task<IList<PurchasedBookReturnDTO>> GetUserBooks(int userId)
        {
            var purchasedBooks = await _dbContext.PurchasedBooks.Where(pb => pb.UserId == userId).ToListAsync();

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
            var pb = await _dbContext.PurchasedBooks.Where(pb => pb.UserId == userId && pb.BookId == bookId).FirstOrDefaultAsync();

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
    }
}
