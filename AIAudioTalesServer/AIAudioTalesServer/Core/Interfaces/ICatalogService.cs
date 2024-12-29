using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Core.Interfaces
{
    public interface ICatalogService
    {
        Task<IList<Book>> GetAllBooksAsync();
        Task<IList<Category>> GetAllCategoriesAsync();
        Task<DTOReturnBook?> GetBookAsync(int bookId);
        Task<IList<DTOReturnBook>> GetBooksFromCategoryAsync(int categoryId, int pageNumber, int pageSize);
        Task<IList<DTOReturnBook>> SearchBooksAsync(string searchTerm, int pageNumber, int pageSize);
        Task<DTOReturnPart?> GetPartAsync(int partId);
        Task<DTOReturnTreePart?> GetBookTreeAsync(int bookId);
        Task<BookPart?> GetRootPart(int bookId);
    }
}
