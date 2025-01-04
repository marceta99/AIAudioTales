using Kumadio.Domain.Entities;

namespace Kumadio.Infrastructure.Interfaces
{
    public interface ICatalogRepository
    {
        Task<IList<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int bookId);
        Task<IList<Book>> GetBooksByCategoryAsync(int categoryId, int skip, int take);
        Task<IList<Category>> GetAllCategoriesAsync();
        Task<IList<Book>> SearchBooksAsync(string searchTerm, int skip, int take);
        Task<BookPart?> GetBookPartAsync(int partId);
        Task<IList<Answer>> GetAnswersForPartAsync(int partId);
        Task<BookPart?> GetRootPartAsync(int bookId);
    }
}
