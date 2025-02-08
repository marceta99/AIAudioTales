using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using static System.Reflection.Metadata.BlobBuilder;

namespace Kumadio.Core.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ICatalogService _catalogService;
        private readonly IBookRepository _bookRepository;
        private readonly IBookPartRepository _bookPartRepository;
        private readonly ISearchHistoryRepository _searchHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPurchasedBookRepository _purchasedBookRepository;

        public LibraryService(
            ICatalogService catalogService,
            IBookRepository bookRepository,
            IBookPartRepository bookPartRepository,
            ISearchHistoryRepository searchHistoryRepository,
            IUnitOfWork unitOfWork,
            IPurchasedBookRepository purchasedBookRepository)
        {
            _catalogService = catalogService;
            _bookRepository = bookRepository;
            _bookPartRepository = bookPartRepository;
            _searchHistoryRepository = searchHistoryRepository;
            _unitOfWork = unitOfWork;
            _purchasedBookRepository = purchasedBookRepository;
        }

        public async Task<Result<bool>> UserHasBook(int bookId, int userId)
        {
            return await _purchasedBookRepository
                .Any(pb => pb.BookId == bookId
                        && pb.UserId == userId
                        && pb.PurchaseStatus == PurchaseStatus.Success);   
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

        public async Task<Result<IList<string>>> GetSearchHistory(int userId)
        {
            var searchHistory = await _searchHistoryRepository.GetSearchHistory(userId);

            return Result<IList<string>>.Success(searchHistory);
        }

        public async Task<Result<PurchasedBook>> GetCurrentBook(int userId)
        {
            var pb = await _purchasedBookRepository.GetCurrentBook(userId);
            if (pb == null) return DomainErrors.Library.CurrentBookNotFound;

            return pb;
        }


        // POST

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

        // PATCH
        public async Task<Result> AddToLibrary(User user, int bookId)
        {
            var book = await _bookRepository.GetFirstWhere(b => b.Id == bookId);
            var rootPart = await _bookPartRepository.GetRootPart(bookId);
            if (book == null || rootPart == null) return DomainErrors.Library.InvalidBook;

            var pb = new PurchasedBook
            {
                BookId = book.Id,
                UserId = user.Id,
                PurchaseType = PurchaseType.Enroled,
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

        public async Task<DTOReturnPurchasedBook?> ActivateQuestionsAsync(int bookId, int userId, decimal playingPosition)
        {
            var pb = await _libraryRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            pb.QuestionsActive = true;
            pb.PlayingPosition = playingPosition;
            await _libraryRepository.UpdatePurchaseAsync(pb);

            var domainBook = await _catalogService.GetBookAsync(pb.BookId);
            if (domainBook == null) return null;

            return new DTOReturnPurchasedBook
            {
                Id = domainBook.Id,
                Description = domainBook.Description,
                Title = domainBook.Title,
                ImageURL = domainBook.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(pb.PlayingPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };
        }

        public async Task<DTOReturnPurchasedBook?> UpdateProgressAsync(DTOUpdateProgress dto, int userId)
        {
            var pb = await _libraryRepository.GetPurchasedBookAsync(userId, dto.BookId);
            if (pb == null) return null;

            if (dto.PlayingPosition.HasValue)
                pb.PlayingPosition = dto.PlayingPosition.Value;

            if (dto.QuestionsActive.HasValue)
                pb.QuestionsActive = dto.QuestionsActive.Value;

            // If there's next book to be played, mark old as false, next as true
            if (dto.NextBookId.HasValue)
            {
                pb.IsBookPlaying = false;
                var nextPb = await _libraryRepository.GetPurchasedBookAsync(userId, dto.NextBookId.Value);
                if (nextPb != null) nextPb.IsBookPlaying = true;

                // Save both
                await _libraryRepository.UpdatePurchasedBooksAsync(new[] { pb, nextPb });
            }
            else
            {
                await _libraryRepository.UpdatePurchaseAsync(pb);
            }

            var domainBook = await _catalogService.GetBookAsync(pb.BookId);
            if (domainBook == null) return null;

            return new DTOReturnPurchasedBook
            {
                Id = domainBook.Id,
                Description = domainBook.Description,
                Title = domainBook.Title,
                ImageURL = domainBook.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(pb.PlayingPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };
        }

        public async Task<DTOReturnPurchasedBook?> StartBookAgainAsync(int bookId, int userId)
        {
            var pb = await _libraryRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            var rootPart = await _catalogService.GetRootPart(bookId);
            if (rootPart == null) return null;

            pb.PlayingPartId = rootPart.Id;
            pb.PlayingPosition = 0;
            pb.IsBookPlaying = false;
            await _libraryRepository.UpdatePurchaseAsync(pb);

            var domainBook = await _catalogService.GetBookAsync(pb.BookId);
            if (domainBook == null) return null;

            return new DTOReturnPurchasedBook
            {
                Id = domainBook.Id,
                Description = domainBook.Description,
                Title = domainBook.Title,
                ImageURL = domainBook.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(rootPart),
                PlayingPosition = 0,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };
        }


        // DELETE
        public async Task<DTOBasket> RemoveBasketItemAsync(int userId, int itemId)
        {
            var item = await _libraryRepository.GetBasketItemByIdAsync(itemId);
            if (item == null)
            {
                // Return current basket anyway
                return await GetBasketAsync(userId);
            }

            await _libraryRepository.RemoveBasketItemAsync(item);
            return await GetBasketAsync(userId);
        }

        public async Task<bool> RemoveUserPendingPurchases(User user)
        {
            return await _libraryRepository.RemoveUserPendingPurchases(user);
        }

        public async Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems,
            PurchaseType purchaseType, Language language, string sessionId)
        {
            var bookIds = new List<int>();

            foreach(var basketItem in basketItems)
            {
                bookIds.Add(basketItem.Id);
            }

            await _libraryRepository.PurchaseBooks(userId, bookIds, purchaseType, language, sessionId);
        }

        public async Task RemoveBasketItems(int userId)
        {
            await _libraryRepository.RemoveBasketItems(userId);
        }

        public async Task<bool> UpdatePurchaseStatus(string sessionId)
        {
            return await _libraryRepository.UpdatePurchaseStatus(sessionId);
        }

        public async Task<bool> RemoveCanceledPurchase(string sessionId)
        {
            return await _libraryRepository.RemoveCanceledPurchase(sessionId);
        }
    }
}
