using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class BookPartRepository : Repository<BookPart>, IBookPartRepository
    {
        public BookPartRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<BookPart?> GetRootPartAsync(int bookId)
        {
            return await _dbSet
                .Where(bp => bp.BookId == bookId && bp.IsRoot == true)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }
    }
}
