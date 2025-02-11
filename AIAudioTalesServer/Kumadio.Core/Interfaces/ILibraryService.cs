using Kumadio.Core.Common;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;

namespace Kumadio.Core.Interfaces
{
    public interface ILibraryService
    {
        #region Library
        Task<Result<bool>> UserHasBook(int bookId, int userId);
        Task<Result<IList<PurchasedBook>>> GetPurchasedBooks(int userId);
        Task<Result<IList<Book>>> GetCreatorBooks(int userId);
        Task<Result<PurchasedBook>> GetPurchasedBook(int userId, int bookId);
        Task<Result<PurchasedBook>> GetCurrentBook(int userId);
        Task<Result> AddToLibrary(User user, int bookId);
        #endregion

        #region Search
        Task<Result> AddSearchTerm(int userId, string searchTerm);
        Task<Result<IList<string>>> GetSearchHistory(int userId);
        #endregion

        #region Book Player
        Task<Result<PurchasedBook>> NextPart(int bookId, int nextPartId, int userId);
        Task<Result<PurchasedBook>> ActivateQuestions(int bookId, int userId, decimal playingPosition);
        Task<Result<PurchasedBook>> UpdateProgress(UpdateProgressModel progressModel, int userId);
        Task<Result<PurchasedBook>> RestartBook(int bookId, int userId);
        #endregion
    }
}
