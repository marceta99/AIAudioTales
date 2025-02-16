using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Common.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IList<Book>> GetBooks(int categoryId, int skip, int take);
        Task<IList<Book>> SearchBooks(string searchTerm, int skip, int take);
    }
}
