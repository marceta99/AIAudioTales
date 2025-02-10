using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Common.Interfaces
{
    public interface IPurchasedBookRepository : IRepository<PurchasedBook>
    {
        Task<IList<PurchasedBook>> GetPurchasedBooks(int userId);
        Task<PurchasedBook?> GetCurrentBook(int userId);
        Task<PurchasedBook?> GetPurchasedBook(int userId, int bookId);
    }
}
