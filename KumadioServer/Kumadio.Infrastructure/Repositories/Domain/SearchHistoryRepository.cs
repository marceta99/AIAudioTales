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
                .Take(15)
                .Select(h => h.SearchTerm)
                .ToListAsync();
        }

        public async Task AddSearchTerm(int userId, string searchTerm)
        {
            // 1) Fetch all existing search history for user, sorted by newest
            var existing = await _dbContext.SearchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.SearchDate)
                .ToListAsync();

            // 2) Check if the exact term already exists; remove it if so
            var duplicate = existing.FirstOrDefault(x =>
                x.SearchTerm.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (duplicate != null)
            {
                _dbContext.SearchHistories.Remove(duplicate);
                existing.Remove(duplicate);
            }

            // 3) If we already have 15 entries, remove the oldest
            if (existing.Count >= 15)
            {
                var oldest = existing.Last(); // because list is sorted desc
                _dbContext.SearchHistories.Remove(oldest);
            }

            // 4) Insert the new search term with current date
            var newSearchHistory = new SearchHistory
            {
                UserId = userId,
                SearchTerm = searchTerm,
                SearchDate = DateTime.UtcNow
            };
            await _dbSet.AddAsync(newSearchHistory);
        }
    }
}
