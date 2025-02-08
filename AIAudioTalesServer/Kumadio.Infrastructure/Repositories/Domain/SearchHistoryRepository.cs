using Kumadio.Core.Common.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Infrastructure.Data;
using Kumadio.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Kumadio.Infrastructure.Repositories.Domain
{
    public class SearchHistoryRepository : Repository<SearchHistory>, ISearchHistoryRepository
    {
        public SearchHistoryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IList<string>> GetSearchHistory(int userId)
        {
            return await _dbContext.SearchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SearchDate)
                .Take(10)
                .Select(h => h.SearchTerm)
                .ToListAsync();
        }
    }
}
