using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class PurchasedBookRepository : Repository<PurchasedBook>, IPurchasedBookRepository
    {
        public PurchasedBookRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<PurchasedBook>> GetPurchasedBooks(int userId)
        {
            return await _dbSet
                .Where(pb => pb.UserId == userId)
                .Include(pb => pb.Book)
                .Include(pb => pb.PlayingPart)
                .ThenInclude(bp => bp.Answers)
                .ToListAsync();
        }

        public async Task<PurchasedBook?> GetCurrentBook(int userId)
        {
            return await _dbSet
                .Where(pb => pb.UserId == userId
                          && pb.IsBookPlaying == true)
                .Include(pb => pb.Book)
                .Include(pb => pb.PlayingPart)
                .ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }
        public async Task<PurchasedBook?> GetPurchasedBook(int userId, int bookId)
        {
            return await _dbSet
                .Where(pb => pb.UserId == userId && pb.BookId == bookId)
                .Include(pb => pb.Book)
                .Include(pb => pb.PlayingPart)
                .ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();
        }

    }
}
