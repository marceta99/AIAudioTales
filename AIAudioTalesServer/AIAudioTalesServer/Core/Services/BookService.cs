// FILE: BooksService.cs
using AIAudioTalesServer.Application.Services;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Domain.Enums;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Web.DTOS;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AIAudioTalesServer.Application.Services
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;  // if you want caching at the service level

        public BooksService(IBooksRepository booksRepository, IMapper mapper, IMemoryCache cache)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
            _cache = cache;
        }

        // GET
        public async Task<IList<Book>> GetAllBooksAsync()
        {
            // Example: place caching here instead of repository
            const string cacheKey = "all_books";
            if (!_cache.TryGetValue(cacheKey, out IList<Book>? books))
            {
                books = await _booksRepository.GetAllBooksAsync();
                _cache.Set(cacheKey, books, TimeSpan.FromMinutes(5));
            }
            return books ?? new List<Book>();
        }

        public async Task<IList<Category>> GetAllCategoriesAsync()
        {
            const string cacheKey = "all_categories";
            if (!_cache.TryGetValue(cacheKey, out IList<Category>? categories))
            {
                categories = await _booksRepository.GetAllCategoriesAsync();
                _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(5));
            }
            return categories ?? new List<Category>();
        }

        public async Task<DTOReturnBook?> GetBookAsync(int bookId)
        {
            var book = await _booksRepository.GetBookByIdAsync(bookId);
            if (book == null) return null;
            return _mapper.Map<DTOReturnBook>(book);
        }

        public async Task<IList<DTOReturnBook>> GetBooksFromCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _booksRepository.GetBooksByCategoryAsync(categoryId, skip, pageSize);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }

        public async Task<bool> UserHasBookAsync(int bookId, int userId)
        {
            return await _booksRepository.UserHasBookAsync(bookId, userId);
        }

        public async Task<bool> IsBasketItemAsync(int bookId, int userId)
        {
            return await _booksRepository.IsBasketItemAsync(bookId, userId);
        }

        public async Task<IList<DTOReturnPurchasedBook>> GetPurchasedBooksAsync(int userId)
        {
            var purchasedBooks = await _booksRepository.GetPurchasedBooksAsync(userId);
            // Map each purchased domain entity to a DTOReturnPurchasedBook
            var results = new List<DTOReturnPurchasedBook>();

            foreach (var pb in purchasedBooks)
            {
                var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
                if (domainBook == null) continue;

                var dto = new DTOReturnPurchasedBook
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
                results.Add(dto);
            }
            return results;
        }

        public async Task<IList<DTOReturnBook>> GetCreatorBooksAsync(int userId)
        {
            var books = await _booksRepository.GetCreatorBooksAsync(userId);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }

        public async Task<DTOReturnPurchasedBook?> GetPurchasedBookAsync(int userId, int bookId)
        {
            var pb = await _booksRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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

        public async Task<IList<DTOReturnBook>> SearchBooksAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _booksRepository.SearchBooksAsync(searchTerm, skip, pageSize);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }

        public async Task<IList<string>> GetSearchHistoryAsync(int userId)
        {
            return await _booksRepository.GetSearchHistoryAsync(userId);
        }

        public async Task<DTOBasket> GetBasketAsync(int userId)
        {
            var basketItems = await _booksRepository.GetBasketItemsAsync(userId);
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
            var pb = await _booksRepository.GetCurrentPurchasedBookAsync(userId);
            if (pb == null) return null;

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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

        // Book Parts / Tree
        public async Task<DTOReturnPart?> GetPartAsync(int partId)
        {
            var part = await _booksRepository.GetBookPartAsync(partId);
            if (part == null) return null;
            return _mapper.Map<DTOReturnPart>(part);
        }

        public async Task<DTOReturnTreePart?> GetBookTreeAsync(int bookId)
        {
            var rootPart = await _booksRepository.GetRootPartAsync(bookId);
            if (rootPart == null) return null;

            // Build the tree
            var rootDto = new DTOReturnTreePart
            {
                PartId = rootPart.Id,
                PartName = "Intro", // or rootPart?.Name if you prefer
                Answers = _mapper.Map<IList<DTOReturnAnswer>>(rootPart.Answers)
            };

            rootDto.NextParts = await PopulateTree(rootPart.Answers);
            return rootDto;
        }

        private async Task<IList<DTOReturnTreePart>> PopulateTree(IList<Answer>? answers)
        {
            if (answers == null) return new List<DTOReturnTreePart>();

            var list = new List<DTOReturnTreePart>();
            foreach (var ans in answers)
            {
                if (ans.NextPartId == null) continue;
                var childPart = await _booksRepository.GetBookPartAsync(ans.NextPartId.Value);
                if (childPart == null) continue;

                var newTree = new DTOReturnTreePart
                {
                    PartId = childPart.Id,
                    PartName = ans.Text,
                    Answers = _mapper.Map<IList<DTOReturnAnswer>>(childPart.Answers)
                };
                newTree.NextParts = await PopulateTree(childPart.Answers);
                list.Add(newTree);
            }
            return list;
        }

        // POST
        public async Task<string?> UploadAsync(IFormFile file, HttpRequest request)
        {
            // e.g. validate file size/extension here
            if (file == null || file.Length == 0) return null;

            // We'll do a basic approach: generate a link
            // For a real scenario, you'd store them in some folder or an S3 bucket, etc.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            // Return the URL
            var audioLink = $"https://{request.Host}/uploads/{fileName}";
            // You can do the actual disk I/O here or in the repository if you prefer “pure data”.
            // For pure DB approach, you could store the file in DB as bytes (less common).
            return audioLink;
        }

        public async Task<DTOReturnPart?> AddRootPartAsync(DTOCreateRootPart root, HttpRequest request)
        {
            // 1) Check if book exists, etc. The repository method is pure DB, so we do logic here
            var domainBook = await _booksRepository.GetBookByIdAsync(root.BookId);
            if (domainBook == null) return null;

            // 2) Upload audio
            var audioLink = await UploadAsync(root.PartAudio, request);
            if (audioLink == null) return null;

            // 3) Create BookPart
            var bookPart = new BookPart
            {
                BookId = root.BookId,
                IsRoot = true,
                PartAudioLink = audioLink
            };
            var createdPart = await _booksRepository.AddBookPartAsync(bookPart);

            // 4) Add answers
            var answersToAdd = new List<Answer>();
            if (root.Answers != null)
            {
                foreach (var a in root.Answers)
                {
                    answersToAdd.Add(new Answer
                    {
                        Text = a.Text,
                        CurrentPartId = createdPart.Id
                    });
                }
                await _booksRepository.AddAnswersAsync(answersToAdd);
                createdPart.Answers = answersToAdd;
            }
            return _mapper.Map<DTOReturnPart>(createdPart);
        }

        public async Task<DTOReturnPart?> AddBookPartAsync(DTOCreatePart part, HttpRequest request)
        {
            // 1) Check if book exists
            var domainBook = await _booksRepository.GetBookByIdAsync(part.BookId);
            if (domainBook == null) return null;

            // 2) Upload audio
            var audioLink = await UploadAsync(part.PartAudio, request);
            if (audioLink == null) return null;

            // 3) Create child BookPart
            var bookPart = new BookPart
            {
                BookId = part.BookId,
                PartAudioLink = audioLink
            };
            var createdPart = await _booksRepository.AddBookPartAsync(bookPart);

            // 4) Link it to parent answer
            // Note that we do not store the “parentAnswer.NextPartId” here
            var parentAnswer = await _booksRepository.GetAnswersForPartAsync(part.ParentAnswerId);
            // Simplify checking, or you can do a dedicated method to fetch a single Answer by ID
            var theParentAnswer = parentAnswer.FirstOrDefault(a => a.Id == part.ParentAnswerId);
            if (theParentAnswer != null && theParentAnswer.NextPartId == null)
            {
                theParentAnswer.NextPartId = createdPart.Id;
                await _booksRepository.UpdateAnswerAsync(theParentAnswer);
                // (We’d need the dbContext or a “repository.UpdateAnswer()” method.)
            }

            // 5) Add child answers if any
            var answersToAdd = new List<Answer>();
            if (part.Answers != null)
            {
                foreach (var ans in part.Answers)
                {
                    if (!string.IsNullOrEmpty(ans.Text))
                    {
                        answersToAdd.Add(new Answer
                        {
                            Text = ans.Text,
                            CurrentPartId = createdPart.Id
                        });
                    }
                }
                await _booksRepository.AddAnswersAsync(answersToAdd);
                createdPart.Answers = answersToAdd;
            }

            return _mapper.Map<DTOReturnPart>(createdPart);
        }

        public async Task<int> AddBookAsync(DTOCreateBook dto, int creatorId)
        {
            var domainBook = new Book
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                CreatorId = creatorId
            };
            var created = await _booksRepository.AddBookAsync(domainBook);
            return created.Id;
        }

        public async Task<DTOBasket> AddBasketItemAsync(int userId, int bookId)
        {
            var basketItem = await _booksRepository.AddBasketItemAsync(userId, bookId);
            if (basketItem == null)
                return new DTOBasket { BasketItems = new List<DTOReturnBasketItem>(), TotalPrice = 0 };

            // Return updated basket
            return await GetBasketAsync(userId);
        }

        public async Task SaveSearchTermAsync(int userId, string searchTerm)
        {
            // You can do checks (like limiting to 15) here or in the repo
            await _booksRepository.AddNewSearchTermAsync(userId, searchTerm);
        }

        // PATCH
        public async Task<bool> AddToLibraryAsync(User user, int bookId)
        {
            var domainBook = await _booksRepository.GetBookByIdAsync(bookId);
            if (domainBook == null) return false;

            return await _booksRepository.AddToLibraryAsync(user, domainBook);
        }

        public async Task<bool> UpdatePurchaseStatusAsync(string sessionId)
        {
            var purchase = await _booksRepository.GetPurchaseBySessionIdAsync(sessionId);
            if (purchase == null) return false;

            purchase.PurchaseStatus = PurchaseStatus.Success;
            await _booksRepository.UpdatePurchaseAsync(purchase);
            return true;
        }

        public async Task<DTOReturnPurchasedBook?> NextPartAsync(DTOUpdateNextPart dto, int userId)
        {
            var pb = await _booksRepository.GetPurchasedBookAsync(userId, dto.BookId);
            if (pb == null) return null;

            pb.PlayingPartId = dto.NextPartId;
            pb.PlayingPosition = 0;
            pb.QuestionsActive = false;
            await _booksRepository.UpdatePurchaseAsync(pb);

            var playingPart = await _booksRepository.GetBookPartAsync(dto.NextPartId);
            if (playingPart == null) return null;

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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
            var pb = await _booksRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            pb.QuestionsActive = true;
            pb.PlayingPosition = playingPosition;
            await _booksRepository.UpdatePurchaseAsync(pb);

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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
            var pb = await _booksRepository.GetPurchasedBookAsync(userId, dto.BookId);
            if (pb == null) return null;

            if (dto.PlayingPosition.HasValue)
                pb.PlayingPosition = dto.PlayingPosition.Value;

            if (dto.QuestionsActive.HasValue)
                pb.QuestionsActive = dto.QuestionsActive.Value;

            // If there's next book to be played, mark old as false, next as true
            if (dto.NextBookId.HasValue)
            {
                pb.IsBookPlaying = false;
                var nextPb = await _booksRepository.GetPurchasedBookAsync(userId, dto.NextBookId.Value);
                if (nextPb != null) nextPb.IsBookPlaying = true;

                // Save both
                await _booksRepository.UpdatePurchasedBooksAsync(new[] { pb, nextPb });
            }
            else
            {
                await _booksRepository.UpdatePurchaseAsync(pb);
            }

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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
            var pb = await _booksRepository.GetPurchasedBookAsync(userId, bookId);
            if (pb == null) return null;

            var rootPart = await _booksRepository.GetRootPartAsync(bookId);
            if (rootPart == null) return null;

            pb.PlayingPartId = rootPart.Id;
            pb.PlayingPosition = 0;
            pb.IsBookPlaying = false;
            await _booksRepository.UpdatePurchaseAsync(pb);

            var domainBook = await _booksRepository.GetBookByIdAsync(pb.BookId);
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
            var item = await _booksRepository.GetBasketItemByIdAsync(itemId);
            if (item == null)
            {
                // Return current basket anyway
                return await GetBasketAsync(userId);
            }

            await _booksRepository.RemoveBasketItemAsync(item);
            return await GetBasketAsync(userId);
        }

        public async Task<bool> RemoveUserPendingPurchases(User user)
        {
            return await _booksRepository.RemoveUserPendingPurchases(user);
        }

        public async Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems,
            PurchaseType purchaseType, Language language, string sessionId)
        {
            await _booksRepository.PurchaseBooks(userId, basketItems, purchaseType, language, sessionId);
        }

        public async Task RemoveBasketItems(int userId)
        {
            await _booksRepository.RemoveBasketItems(userId);
        }

        public async Task<bool> UpdatePurchaseStatus(string sessionId)
        {
            return await _booksRepository.UpdatePurchaseStatus(sessionId);
        }

        public async Task<bool> RemoveCanceledPurchase(string sessionId)
        {
            return await _booksRepository.RemoveCanceledPurchase(sessionId);
        }
    }
}
