using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class BookPartRepository : Repository<BookPart>, IBookPartRepository
    {
        public BookPartRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<BookPart?> GetRootPart(int bookId)
        {
            return await _dbSet
                .Where(bp => bp.BookId == bookId && bp.IsRoot == true)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }

        public override async Task<BookPart?> GetFirstWhere(Expression<Func<BookPart, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }
    }
}
