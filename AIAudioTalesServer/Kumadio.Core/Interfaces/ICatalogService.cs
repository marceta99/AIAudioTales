using Kumadio.Core.Common;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface ICatalogService
    {
        #region Books
        Task<Result<Book>> GetBook(int bookId);
        Task<Result<IList<Book>>> GetBooks(int categoryId, int pageNumber, int pageSize);
        Task<Result<IList<Book>>> SearchBooks(string searchTerm, int pageNumber, int pageSize);
        
        #endregion

        #region Parts
        Task<Result<BookPart>> GetPart(int partId);
        Task<Result<PartTree>> GetPartTree(int bookId);
        #region

        Task<Result<IList<Category>>> GetAllCategories();
    }
}
