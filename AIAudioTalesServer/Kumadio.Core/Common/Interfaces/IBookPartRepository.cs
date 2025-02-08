using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Common.Interfaces
{
    public interface IBookPartRepository : IRepository<BookPart>
    {
        Task<BookPart?> GetRootPart(int bookId);
    }
}
