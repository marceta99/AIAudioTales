using Kumadio.Core.Common;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface IEditorService
    {
        Task<Result<BookPart>> AddRootPart(RootPartModel root, string host);
        Task<Result<BookPart>> AddBookPart(PartModel part, string host);
        Task<Result<Book>> AddBook(Book book);
    }
}
