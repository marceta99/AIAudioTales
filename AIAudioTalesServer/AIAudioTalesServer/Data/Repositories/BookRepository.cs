using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<BookReturnDTO> AddNewBook(BookCreationDTO newBook)
        {
            var book = _mapper.Map<Book>(newBook);
            var createdBook = _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();

            var returnBook = _mapper.Map<BookReturnDTO>(createdBook.Entity);
            return returnBook;
        }
        public async Task<int> DeleteBook(int bookId)
        {
            var bookToDelete = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();
            if (bookToDelete == null)
            {
                return 0;
            }
            _dbContext.Books.Remove(bookToDelete);
            return await _dbContext.SaveChangesAsync();
        }
        public async Task<IList<BookReturnDTO>> GetAllBooks()
        {
            var books = await _dbContext.Books.ToListAsync();
            var returnBooks = _mapper.Map<IList<BookReturnDTO>>(books);
            return returnBooks;
        }
        public async Task<BookReturnDTO> GetBook(int id)
        {
            var book = await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
            
            var returnBook = _mapper.Map<BookReturnDTO>(book);
            return returnBook;
        }
        public async Task<IList<BookReturnDTO>> GetBooksForCategory(BookCategory bookCategory)
        {
            var books = await _dbContext.Books.Where(b => b.BookCategory == bookCategory).ToListAsync();
            var returnBooks = _mapper.Map<IList<BookReturnDTO>>(books);

            return returnBooks;
        }
        public async Task<IList<BookReturnDTO>?> GetBooksFromSpecificUser(int userId)
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null) return null;

            var purchasedBooks = await _dbContext.PurchasedBooks.Where(pb => pb.UserId == userId).ToListAsync();

            List<BookReturnDTO> books = new List<BookReturnDTO>();

            foreach(var pb in purchasedBooks)
            {
                var book = await GetBook(pb.BookId);
                book.PurchaseType = pb.PurchaseType;
                book.Language = pb.Language;
                books.Add(book);
            }
     
            return books;
        }
        public async Task<int> UpdateBookDetails(BookUpdateDTO book)
        {
            var bookToEdit = await _dbContext.Books.Where(b => b.Id == book.Id).FirstOrDefaultAsync();
            if (bookToEdit == null)
            {
                return 0;
            }
            bookToEdit.Title = book.Title;
            bookToEdit.Description = book.Description;
            return await _dbContext.SaveChangesAsync();
        
        }
        public async Task<int> UploadImageForBook(int bookId, IFormFile imageFile)
        {
            var bookToEdit = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();
            if (bookToEdit == null)
            {
                return 0;
            }

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                bookToEdit.ImageData = imageBytes;

                return await _dbContext.SaveChangesAsync();
            }

        }
        
        public async Task<byte[]?> GetBookImage(int bookId)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return null;

            return book.ImageData;
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
    }
}
