using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Domain.Enums;
using static System.Reflection.Metadata.BlobBuilder;

namespace Kumadio.Core.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ICatalogService _catalogService;
        private readonly IPurchasedBookRepository _purchasedBookRepository;

        public LibraryService(ICatalogService catalogService, IPurchasedBookRepository purchasedBookRepository)
        {
            _catalogService = catalogService;
            _purchasedBookRepository = purchasedBookRepository;
        }

        // GET

        public async Task<Result<bool>> UserHasBook(int bookId, int userId)
        {
            return await _purchasedBookRepository.Any(pb => pb.BookId == bookId
                                                          && pb.UserId == userId
                                                          && pb.PurchaseStatus == PurchaseStatus.Success);   
        }

        public async Task<Result<IList<PurchasedBook>>> GetPurchasedBooks(int userId)
        {
            var purchasedBooks = await _purchasedBookRepository.GetPurchasedBooks(userId);

            return Result<IList<PurchasedBook>>.Success(purchasedBooks);
        }

        public async Task<IList<DTOReturnBook>> GetCreatorBooksAsync(int userId)
        {
            var books = await _libraryRepository.GetCreatorBooksAsync(userId);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }

        public async Task<DTOReturnPurchasedBook?> GetPurchasedBookAsync(int userId, int bookId)
        {
            var pb = await _libraryRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            var domainBook = await _catalogService.GetBookAsync(pb.BookId);
            if (domainBook == null) return null;

            return new DTOReturnPurchasedBook
            {
                Id = domainBook.Id,
                Description = domainBook.Description,
                Title = domainBook.Title,
                ImageURL = domainBook.ImageURL,
                CategoryId = domainBook.CategoryId,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language
            };
        }

        public async Task<IList<string>> GetSearchHistoryAsync(int userId)
        {
            return await _libraryRepository.GetSearchHistoryAsync(userId);
        }

        public async Task<DTOBasket> GetBasketAsync(int userId)
        {
            var basketItems = await _libraryRepository.GetBasketItemsAsync(userId);
            var itemDtos = _mapper.Map<IList<DTOReturnBasketItem>>(basketItems);
            var total = itemDtos.Sum(i => i.ItemPrice);

            return new DTOBasket
            {
                BasketItems = itemDtos,
                TotalPrice = total
            };
        }

        public async Task<DTOReturnPurchasedBook?> GetCurrentBookAsync(int userId)
        {
            var pb = await _libraryRepository.GetCurrentPurchasedBookAsync(userId);
            if (pb == null) return null;

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


        // POST


        public async Task<DTOBasket> AddBasketItemAsync(int userId, int bookId)
        {
            var basketItem = await _libraryRepository.AddBasketItemAsync(userId, bookId);
            if (basketItem == null)
                return new DTOBasket { BasketItems = new List<DTOReturnBasketItem>(), TotalPrice = 0 };

            // Return updated basket
            return await GetBasketAsync(userId);
        }

        public async Task SaveSearchTermAsync(int userId, string searchTerm)
        {
            // You can do checks (like limiting to 15) here or in the repo
            await _libraryRepository.AddNewSearchTermAsync(userId, searchTerm);
        }

        // PATCH
        public async Task<bool> AddToLibraryAsync(User user, int bookId)
        {

            return await _libraryRepository.AddToLibraryAsync(user, bookId);
        }

        public async Task<bool> UpdatePurchaseStatusAsync(string sessionId)
        {
            var purchase = await _libraryRepository.GetPurchaseBySessionIdAsync(sessionId);
            if (purchase == null) return false;

            purchase.PurchaseStatus = PurchaseStatus.Success;
            await _libraryRepository.UpdatePurchaseAsync(purchase);
            return true;
        }


        public async Task<DTOReturnPurchasedBook?> NextPartAsync(DTOUpdateNextPart dto, int userId)
        {
            var pb = await _libraryRepository.GetPurchasedBookAsync(userId, dto.BookId);
            if (pb == null) return null;

            pb.PlayingPartId = dto.NextPartId;
            pb.PlayingPosition = 0;
            pb.QuestionsActive = false;
            await _libraryRepository.UpdatePurchaseAsync(pb);

            var playingPart = await _catalogService.GetPartAsync(dto.NextPartId);
            if (playingPart == null) return null;

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
                PlayingPart = _mapper.Map<DTOReturnPart>(playingPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };
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
