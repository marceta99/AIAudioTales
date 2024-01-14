using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.Enums;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public BooksRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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
                PurchaseType = pb.PurchaseType,
                Language = pb.Language
            };

            return purchasedBook;
        }


    }
}
