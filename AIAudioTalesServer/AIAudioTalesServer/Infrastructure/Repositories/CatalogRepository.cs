using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Infrastructure.Data;
using AIAudioTalesServer.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly AppDbContext _dbContext;
        public CatalogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Book>> GetAllBooksAsync()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<IList<Category>> GetAllCategoriesAsync()
        {
            return await _dbContext.BookCategories.ToListAsync();
        }

        public async Task<IList<Answer>> GetAnswersForPartAsync(int partId)
        {
            return await _dbContext.Answers
                .Where(a => a.CurrentPartId == partId)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            return await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<BookPart?> GetBookPartAsync(int partId)
        {
            return await _dbContext.BookParts
                .Include(bp => bp.Answers)
                .Include(bp => bp.ParentAnswer)
                .FirstOrDefaultAsync(bp => bp.Id == partId);
        }
        public async Task<IList<Book>> GetBooksByCategoryAsync(int categoryId, int skip, int take)
        {
            return await _dbContext.Books
                .Where(b => b.CategoryId == categoryId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<BookPart?> GetRootPartAsync(int bookId)
        {
            return await _dbContext.BookParts
                .Where(bp => bp.BookId == bookId && bp.IsRoot == true)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<Book>> SearchBooksAsync(string searchTerm, int skip, int take)
        {
            return await _dbContext.Books
                .Where(b => b.Title.Contains(searchTerm))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
