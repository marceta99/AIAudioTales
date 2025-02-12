using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Interfaces;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;

namespace Kumadio.Core.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookPartRepository _bookPartRepository;
        private readonly ISearchHistoryRepository _searchHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPurchasedBookRepository _purchasedBookRepository;

        public LibraryService(
            IBookRepository bookRepository,
            IBookPartRepository bookPartRepository,
            ISearchHistoryRepository searchHistoryRepository,
            IUnitOfWork unitOfWork,
            IPurchasedBookRepository purchasedBookRepository)
        {
            _bookRepository = bookRepository;
            _bookPartRepository = bookPartRepository;
            _searchHistoryRepository = searchHistoryRepository;
            _unitOfWork = unitOfWork;
            _purchasedBookRepository = purchasedBookRepository;
        }

        #region Library
        public async Task<Result> AddToLibrary(User user, int bookId)
        {
            var book = await _bookRepository.GetFirstWhere(b => b.Id == bookId);
            var rootPart = await _bookPartRepository.GetRootPart(bookId);
            if (book == null || rootPart == null) return DomainErrors.Library.InvalidBook;

            var pb = new PurchasedBook
            {
                BookId = book.Id,
                UserId = user.Id,
                PurchaseType = PurchaseType.Basic,
                Language = Language.ENGLISH_UK,
                PlayingPartId = rootPart.Id,
                PlayingPosition = 0,
                IsBookPlaying = false,
                QuestionsActive = false
            };

            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                await _purchasedBookRepository.Add(pb);

                return Result.Success();
            });
        }
        public async Task<Result<bool>> UserHasBook(int bookId, int userId)
        {
            return await _purchasedBookRepository
                .Any(pb => pb.BookId == bookId
                        && pb.UserId == userId);
        }
        public async Task<Result<IList<PurchasedBook>>> GetPurchasedBooks(int userId)
        {
            var purchasedBooks = await _purchasedBookRepository.GetPurchasedBooks(userId);

            return Result<IList<PurchasedBook>>.Success(purchasedBooks);
        }
        public async Task<Result<IList<Book>>> GetCreatorBooks(int userId)
        {
            var books = await _bookRepository.GetAllWhere(b => b.CreatorId == userId);

            return Result<IList<Book>>.Success(books);
        }
        public async Task<Result<PurchasedBook>> GetPurchasedBook(int userId, int bookId)
        {
            var pb = await _purchasedBookRepository
                            .GetFirstWhere(pb => pb.UserId == userId
                                                && pb.BookId == bookId);

            if (pb == null) return DomainErrors.Library.PurchasedBookNotFound;

            return pb;
        }
        public async Task<Result<PurchasedBook>> GetCurrentBook(int userId)
        {
            var pb = await _purchasedBookRepository.GetCurrentBook(userId);
            if (pb == null) return DomainErrors.Library.CurrentBookNotFound;

            return pb;
        }
        #endregion

        #region Search
        public async Task<Result<IList<string>>> GetSearchHistory(int userId)
        {
            var searchHistory = await _searchHistoryRepository.GetSearchHistory(userId);

            return Result<IList<string>>.Success(searchHistory);
        }
        public async Task<Result> AddSearchTerm(int userId, string searchTerm)
        {
            // You can do checks (like limiting to 15) here or in the repo
            var search = new SearchHistory
            {
                UserId = userId,
                SearchTerm = searchTerm,
                SearchDate = DateTime.UtcNow
            };

            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                await _searchHistoryRepository.Add(search);

                return Result.Success();
            });
        }
        #endregion

        #region Book Player
        public async Task<Result<PurchasedBook>> NextPart(int bookId, int nextPartId, int userId)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var pb = await _purchasedBookRepository.GetPurchasedBook(userId, bookId);
                if (pb == null) return DomainErrors.Library.PurchasedBookNotFound;

                var playingPart = await _bookPartRepository.GetFirstWhere(p => p.Id == nextPartId);
                if (playingPart == null) return DomainErrors.Library.NextPartNotFound;

                pb.PlayingPartId = nextPartId;
                pb.PlayingPosition = 0;
                pb.QuestionsActive = false;

                pb.PlayingPart = playingPart;

                return Result<PurchasedBook>.Success(pb);
            });
        }
        public async Task<Result<PurchasedBook>> ActivateQuestions(int bookId, int userId, decimal playingPosition)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var pb = await _purchasedBookRepository.GetPurchasedBook(userId, bookId);
                if (pb == null) return DomainErrors.Library.PurchasedBookNotFound;

                pb.QuestionsActive = true;
                pb.PlayingPosition = playingPosition;

                return Result<PurchasedBook>.Success(pb);
            });
        }
        public async Task<Result<PurchasedBook>> UpdateProgress(UpdateProgressModel progressModel, int userId)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var pb = await _purchasedBookRepository.GetPurchasedBook(userId, progressModel.BookId);
                if (pb == null) return DomainErrors.Library.PurchasedBookNotFound;

                if (progressModel.PlayingPosition.HasValue)
                    pb.PlayingPosition = progressModel.PlayingPosition.Value;

                if (progressModel.QuestionsActive.HasValue)
                    pb.QuestionsActive = progressModel.QuestionsActive.Value;

                // If there's next book to be played, mark old as false, next as true
                if (progressModel.NextBookId.HasValue)
                {
                    pb.IsBookPlaying = false;
                    var nextPb = await _purchasedBookRepository.GetPurchasedBook(userId, progressModel.NextBookId.Value);
                    if (nextPb == null) return DomainErrors.Library.NextBookNotFound;

                    nextPb.IsBookPlaying = true;
                }

                return Result<PurchasedBook>.Success(pb);
            });
        }
        public async Task<Result<PurchasedBook>> RestartBook(int bookId, int userId)
        {
            return await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var pb = await _purchasedBookRepository.GetPurchasedBook(userId, bookId);
                if (pb == null) return DomainErrors.Library.PurchasedBookNotFound;

                var rootPart = await _bookPartRepository.GetRootPart(bookId);
                if (rootPart == null) return DomainErrors.Common.RootPartNotFound;

                pb.PlayingPartId = rootPart.Id;
                pb.PlayingPosition = 0;
                pb.IsBookPlaying = false;

                pb.PlayingPart = rootPart;

                return Result<PurchasedBook>.Success(pb);
            });
        }
        #endregion
    }
}
