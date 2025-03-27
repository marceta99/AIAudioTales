using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Common.Interfaces
{
    public interface ISearchHistoryRepository : IRepository<SearchHistory>
    {
        Task<IList<string>> GetSearchHistory(int userId);
        Task AddSearchTerm(int userId, string searchTerm);
    }
}
