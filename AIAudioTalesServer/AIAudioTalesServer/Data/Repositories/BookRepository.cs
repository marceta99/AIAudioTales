using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;
        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task AddNewBook(Book book)
        {
            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
        }

        public Task DeleteBook(int bookId)
        {
            throw new NotImplementedException();
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

        public async Task<IList<Book>> GetBooksForCategory(BookCategory bookCategory)
        {
            var books = await _dbContext.Books.Where(b => b.BookCategory == bookCategory).ToListAsync();
            return books;
        }

        public async Task<IList<Book>> GetBooksFromSpecificUser(int userId)
        {
            var purchasedBooks = await _dbContext.PurchasedBooks.Where(pb => pb.UserId == userId).ToListAsync();

            List<Book> books = new List<Book>();

            foreach(var pb in purchasedBooks)
            {
                books.Add(pb.Book);
            }
            return books;
        }

        public async Task UpdateBookDetails(Book book)
        {
            var bookToEdit = await _dbContext.Books.Where(b => b.Id == book.Id).FirstOrDefaultAsync();
            bookToEdit.Title = book.Title;
            bookToEdit.Description = book.Description;
            await _dbContext.SaveChangesAsync();
        
        }
    }
}
