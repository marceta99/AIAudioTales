using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IList<Book>> GetBooks(int categoryId, int skip, int take)
        {
            return await _dbSet
                .Where(b => b.CategoryId == categoryId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<IList<Book>> SearchBooks(string searchTerm, int skip, int take)
        {
            return await _dbSet
                .Where(b => b.Title.Contains(searchTerm))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
