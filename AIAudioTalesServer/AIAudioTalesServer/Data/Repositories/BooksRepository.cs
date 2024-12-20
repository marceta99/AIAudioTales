﻿using AIAudioTalesServer.Models;
using AutoMapper;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.Enums;
using Microsoft.Extensions.Caching.Memory;
using static System.Reflection.Metadata.BlobBuilder;
using Google.Apis.Logging;
using System.Net;
using System.Drawing.Printing;

namespace AIAudioTalesServer.Data.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public BooksRepository(AppDbContext dbContext, IMapper mapper, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _cache = cache;
        }

        #region GET

        public async Task<IList<Book>> GetAllBooks()
        {
            var books = await _dbContext.Books.ToListAsync();
            return books;
        }

        public async Task<DTOReturnBook> GetBook(int id)
        {
            var book = await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();

            var returnBook = _mapper.Map<DTOReturnBook>(book);

            return returnBook;
        }

        public async Task<IList<DTOReturnBook>> GetBooksFromCategory(int categoryId, int pageNumber, int pageSize)
        {
            string cacheKey = $"Search_{categoryId}_{pageNumber}_{pageSize}";
            if (!_cache.TryGetValue(cacheKey, out IList<Book> books))
            {
                books = await _dbContext.Books
                                       .Where(b => b.CategoryId == categoryId)
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, books, cacheEntryOptions);
            }

            var returnBooks = _mapper.Map<IList<DTOReturnBook>>(books);

            return returnBooks;
        }

        public async Task<bool> UserHasBook(int bookId, int userId)
        {
            var purchase = await _dbContext.PurchasedBooks.Where(pb =>
                                                                 pb.BookId == bookId &&
                                                                 pb.UserId == userId &&
                                                                 pb.PurchaseStatus == PurchaseStatus.Success)
                                                                .FirstOrDefaultAsync();
            if (purchase == null) return false;
            return true;
        }

        public async Task<bool> IsBasketItem(int bookId, int userId)
        {
            var basketItem = await _dbContext.BasketItems.Where(bi => bi.UserId == userId && bi.BookId == bookId).FirstOrDefaultAsync();
            if (basketItem == null) return false;
            return true;
        }

        public async Task<IList<DTOReturnPurchasedBook>> GetPurchasedBooks(int userId)
        {
            var purchasedBooks = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb=> pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .ToListAsync();

            List<DTOReturnPurchasedBook> books = new List<DTOReturnPurchasedBook>();

            foreach (var pb in purchasedBooks)
            {
                var book = await GetBook(pb.BookId); //type Book
                var purchasedBook = new DTOReturnPurchasedBook
                {
                    Id = book.Id,
                    Description = book.Description,
                    Title = book.Title,
                    ImageURL = book.ImageURL,
                    PurchaseType = pb.PurchaseType,
                    Language = pb.Language,
                    PlayingPart = _mapper.Map<DTOReturnPart>(pb.PlayingPart),
                    PlayingPosition = pb.PlayingPosition,
                    IsBookPlaying = pb.IsBookPlaying,
                    QuestionsActive = pb.QuestionsActive
                };

                books.Add(purchasedBook);
            }

            return books;
        }

        public async Task<DTOReturnPurchasedBook> GetCurrentBook(int userId)
        {
            var pb = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.IsBookPlaying == true && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb => pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();

            var book = await GetBook(pb.BookId); 
            var purchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(pb.PlayingPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };
        
            return purchasedBook;
        }

        public async Task<IList<DTOReturnBook>> GetCreatorBooks(int userId)
        {
            var books = await _dbContext.Books.Where(b => b.CreatorId == userId).ToListAsync();
            var returnBooks = _mapper.Map<IList<DTOReturnBook>>(books);

            return returnBooks;
        }

        public async Task<DTOReturnPurchasedBook> GetPurchasedBook(int userId, int bookId)
        {
            var pb = await _dbContext.PurchasedBooks.Where(
                                                    pb => pb.UserId == userId &&
                                                    pb.BookId == bookId &&
                                                    pb.PurchaseStatus == PurchaseStatus.Success)
                                                    .FirstOrDefaultAsync();

            if (pb == null) return null;

            var book = await GetBook(pb.BookId);

            var purchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                CategoryId = book.CategoryId,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language
            };

            return purchasedBook;
        }

        public async Task<IList<DTOReturnBook>> SearchBooks(string searchTerm, int pageNumber, int pageSize)
        {
            string cacheKey = $"Search_{searchTerm}_{pageNumber}_{pageSize}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Book> books))
            {
                books = await _dbContext.Books
                                       .Where(b => b.Title.Contains(searchTerm))
                                       .Skip((pageNumber - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, books, cacheEntryOptions);
            }

            var returnBooks = _mapper.Map<IList<DTOReturnBook>>(books);

            return returnBooks;
        }

        public async Task<IList<string>> GetSearchHistory(int userId)
        {
            var history = await _dbContext.SearchHistories
                                    .Where(h => h.UserId == userId)
                                    .OrderByDescending(h => h.SearchDate)
                                    .Take(10)
                                    .Select(h => h.SearchTerm)
                                    .ToListAsync();

            return history;
        }

        public async Task<BasketItem?> GetItemById(int itemId)
        {
            return await _dbContext.BasketItems.FindAsync(itemId);
        }

        public async Task<IList<Category>> GetAllCategories()
        {
            var categories = await _dbContext.BookCategories.ToListAsync();
            return categories;
        }

        public async Task<DTOBasket> GetBasket(int userId)
        {
            var basketItems = await _dbContext.BasketItems
                                              .Where(bi => bi.UserId == userId)
                                              .Include(bi => bi.Book)
                                              .ToListAsync();
            var itemsDto = _mapper.Map<IList<DTOReturnBasketItem>>(basketItems);
            var totalPrice = 0m;

            foreach (var item in itemsDto)
            {
                totalPrice += item.ItemPrice;
            }

            var basket = new DTOBasket
            {
                BasketItems = itemsDto,
                TotalPrice = totalPrice
            };

            return basket;
        }

        public async Task<DTOReturnPart?> GetPart(int partId)
        {
            var part = await _dbContext.BookParts
                .Where(bp => bp.Id == partId)
                .Include(bp => bp.Answers)
                .Include(bp => bp.ParentAnswer)
                .FirstOrDefaultAsync();

            if (part == null) return null;

            var partDto = _mapper.Map<DTOReturnPart>(part);
            return partDto;
        }

        public async Task<DTOReturnTreePart> GetBookTree(int bookId)
        {
            var rootPart = await _dbContext.BookParts
                .Where(bp => bp.BookId == bookId && bp.IsRoot == true)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();

            if (rootPart == null) return null;

            var rootTreePart = new DTOReturnTreePart()
            {
                PartId = rootPart.Id,
                PartName = "Intro",
                Answers = _mapper.Map<IList<DTOReturnAnswer>>(rootPart.Answers)
            };

            rootTreePart.NextParts = await PopulateRootTreeParts(rootPart.Answers);
            
            return rootTreePart;
        }

        private async Task<IList<DTOReturnTreePart>> PopulateRootTreeParts(IList<Answer> answerList)
        {
            if (answerList == null) return null;

            var treeParts = new List<DTOReturnTreePart>();

            foreach (var answer in answerList)
            {
                if(answer.NextPartId == null) continue;

                var newTreePart = new DTOReturnTreePart()
                {
                    PartId = (int)answer.NextPartId,
                    PartName = answer.Text
                };

                var answers = await GetPartAnswers((int)answer.NextPartId);

                newTreePart.Answers = _mapper.Map<IList<DTOReturnAnswer>>(answers);

                newTreePart.NextParts = await PopulateRootTreeParts(answers);

                treeParts.Add(newTreePart);
            }

            return treeParts;
        }

        private async Task<IList<Answer>> GetPartAnswers(int partId)
        {
            var answers = await _dbContext.Answers.Where(a => a.CurrentPartId == partId).ToListAsync();

            return answers;
        }

        #endregion

        #region POST  

        public async Task<string?> Upload(IFormFile file, HttpRequest request)
        {
            if (file != null && file.Length > 0)
            {

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var audioLink = $"https://34b910nm-44320.euw.devtunnels.ms/uploads/{fileName}";
                return audioLink;
            }

            return null;
        }

        public async Task<DTOReturnPart?> AddRootPart(DTOCreateRootPart root, HttpRequest request)
        {
            var book = await _dbContext.Books.Where(b => b.Id == root.BookId)
                .Include(b => b.BookParts)
                .FirstOrDefaultAsync();

            if (book == null || book.BookParts.Count != 0) return null;  // return null if book already has some parts, then there is no point of adding root part 

            var partAudioLink = await Upload(root.PartAudio, request);

            if (partAudioLink == null) return null;

            var bookPart = new BookPart
            {
                BookId = root.BookId,
                IsRoot = true,
                PartAudioLink = partAudioLink
            };

            var createdRootPart = await _dbContext.BookParts.AddAsync(bookPart);

            await _dbContext.SaveChangesAsync();

            if(root.Answers != null)
            {
                IList<Answer> answers = new List<Answer>();
                foreach (var answer in root.Answers)
                {
                    var a = new Answer
                    {
                        Text = answer.Text,
                        CurrentPartId = createdRootPart.Entity.Id
                    };
                    var createdAnswer = await _dbContext.Answers.AddAsync(a);
                    answers.Add(createdAnswer.Entity);
                }
                await _dbContext.SaveChangesAsync();

                createdRootPart.Entity.Answers = answers;
            }

            var partDto = _mapper.Map<DTOReturnPart>(createdRootPart.Entity);

            return partDto;
        }

        public async Task<DTOReturnPart?> AddBookPart(DTOCreatePart part, HttpRequest request)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var book = await _dbContext.Books.Where(b => b.Id == part.BookId)
                .Include(b => b.BookParts)
                .FirstOrDefaultAsync();

                if (book == null) return null;
                
                var partAudioLink = await Upload(part.PartAudio, request);

                if (partAudioLink == null) return null;

                var bookPart = new BookPart
                {
                    BookId = part.BookId,
                    PartAudioLink = partAudioLink
                };

                var createdPart = await _dbContext.BookParts.AddAsync(bookPart);

                await _dbContext.SaveChangesAsync();

                //update the parent answer so that now references child part 
                var parentAnswer = await _dbContext.Answers.Where(a => a.Id == part.ParentAnswerId).FirstOrDefaultAsync();

                if (parentAnswer != null && parentAnswer.NextPartId == null) // if answer already has nextPartId selected then that new part can not be added as next part of that answer and that is why I have additional checkok parentAnswer.nextPartId != 0 because it should be null
                {
                    parentAnswer.NextPartId = createdPart.Entity.Id;

                    await _dbContext.SaveChangesAsync();

                    if(part.Answers != null)
                    {
                        // new answers for child parts
                        IList<Answer> answers = new List<Answer>();
                        foreach (var answer in part.Answers)
                        {
                            if (string.IsNullOrEmpty(answer.Text)) continue; // don't allow empty string to be added as answer

                            var a = new Answer
                            {
                                Text = answer.Text,
                                CurrentPartId = createdPart.Entity.Id
                            };
                            var createdAnswer = await _dbContext.Answers.AddAsync(a);
                            answers.Add(createdAnswer.Entity);
                        }
                        await _dbContext.SaveChangesAsync();

                        createdPart.Entity.Answers = answers;
                    }

                    await transaction.CommitAsync();

                    var partDto = _mapper.Map<DTOReturnPart>(createdPart.Entity);
                    return partDto;
                }

                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Book> AddBook(DTOCreateBook book, int creatorId)
        {
            Book newBook = _mapper.Map<Book>(book);
            newBook.CreatorId = creatorId;

            var createdBook = await _dbContext.Books.AddAsync(newBook);

            await _dbContext.SaveChangesAsync();

            return createdBook.Entity;
        }

        public async Task PurchaseBooks(int userId, IList<DTOReturnBasketItem> basketItems, PurchaseType purchaseType, Language language, string sessionId)
        {
            List<PurchasedBooks> purchasedBooks = new List<PurchasedBooks>();
            foreach (DTOReturnBasketItem basketItem in basketItems)
            {
                PurchasedBooks pb = new PurchasedBooks
                {
                    BookId = basketItem.BookId,
                    UserId = userId,
                    PurchaseType = purchaseType,
                    Language = language,
                    PurchaseStatus = PurchaseStatus.Pending,
                    SessionId = sessionId,
                    PlayingPartId = await GetRootPart(basketItem.BookId),
                    PlayingPosition = 0,
                    IsBookPlaying = false,
                    QuestionsActive = false
                };
                purchasedBooks.Add(pb);
            }
            
            await _dbContext.PurchasedBooks.AddRangeAsync(purchasedBooks);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<int> GetRootPart(int bookId)
        {
            var rootPart = await _dbContext.BookParts.Where(p => p.BookId ==  bookId && p.IsRoot == true).FirstOrDefaultAsync();

            return rootPart.Id;
        }

        public async Task SaveSearchTerm(int userId, string searchTerm)
        {
            // Check if the same search term already exists for this user
            var existingTerm = await _dbContext.SearchHistories
                                             .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.SearchTerm == searchTerm);

            if (existingTerm == null)
            {
                // Get the count of search terms for this user
                var count = await _dbContext.SearchHistories.CountAsync(sh => sh.UserId == userId);

                // If there are already 15 search terms, remove the oldest one
                if (count >= 15)
                {
                    var oldestSearchTerm = await _dbContext.SearchHistories
                                                         .Where(sh => sh.UserId == userId)
                                                         .OrderBy(sh => sh.SearchDate)
                                                         .FirstOrDefaultAsync();
                    if (oldestSearchTerm != null)
                    {
                        _dbContext.SearchHistories.Remove(oldestSearchTerm);
                    }
                }

                // Add the new search term
                var newSearchHistory = new SearchHistory
                {
                    UserId = userId,
                    SearchTerm = searchTerm,
                    SearchDate = DateTime.UtcNow
                };
                _dbContext.SearchHistories.Add(newSearchHistory);

                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task<BasketItem> AddBasketItem(int userId, int bookId)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return null;

            var basketItem = new BasketItem
            {
                UserId = userId,
                BookId = bookId,
                ItemPrice = book.Price
            };

            var item = await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();

            return item.Entity;
        }

        #endregion

        #region PATCH
        public async Task<bool> AddToLibrary(User user, int bookId)
        {
            if (user.Role != Role.LISTENER_WITH_SUBSCRIPTION) return false;

            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return false;

            PurchasedBooks pb = new PurchasedBooks
            {
                BookId = bookId,
                UserId = user.Id,
                PurchaseType = PurchaseType.Enroled,
                Language = Language.ENGLISH_UK,
                PurchaseStatus = PurchaseStatus.Success,
                SessionId = "",
                PlayingPartId = await GetRootPart(bookId),
                PlayingPosition = 0,
                IsBookPlaying = false,
                QuestionsActive = false
            };

            await _dbContext.PurchasedBooks.AddAsync(pb);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdatePurchaseStatus(string sessionId)
        {
            try
            {
                var purchase = await _dbContext.PurchasedBooks.Where(pb => pb.SessionId == sessionId).FirstOrDefaultAsync();

                if (purchase != null)
                {
                    purchase.PurchaseStatus = PurchaseStatus.Success;
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate success
                }
                return false; // No purchase found to update
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false; // Indicate failure
            }
        }

        public async Task<DTOReturnPurchasedBook?> NextPart(DTOUpdateNextPart nextPart, int userId)
        {
            var pb = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.BookId == nextPart.BookId && pb.PurchaseStatus == PurchaseStatus.Success)
                .FirstOrDefaultAsync();

            if (pb == null) return null;

            pb.PlayingPartId = nextPart.NextPartId;
            pb.PlayingPosition = 0;
            pb.QuestionsActive = false;

            await _dbContext.SaveChangesAsync();

            var playingPart = await _dbContext.BookParts
                .Where(bp => bp.Id == pb.PlayingPartId)
                .Include(bp => bp.Answers)
                .FirstOrDefaultAsync();
            
            if (playingPart == null) return null;

            var book = await GetBook(pb.BookId); 
            var purchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(playingPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };

            return purchasedBook;
        }

        public async Task<DTOReturnPurchasedBook?> ActivateQuestions(int bookId, int userId, decimal playingPosition)
        {
            var purchasedBook = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.BookId == bookId && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb => pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();

            if (purchasedBook == null) return null;

            purchasedBook.QuestionsActive = true;
            purchasedBook.PlayingPosition = playingPosition;
            await _dbContext.SaveChangesAsync();

            var book = await GetBook(purchasedBook.BookId);
            var returnPurchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                PurchaseType = purchasedBook.PurchaseType,
                Language = purchasedBook.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(purchasedBook.PlayingPart),
                PlayingPosition = purchasedBook.PlayingPosition,
                IsBookPlaying = purchasedBook.IsBookPlaying,
                QuestionsActive = purchasedBook.QuestionsActive
            };

            return returnPurchasedBook;
        }

        public async Task<DTOReturnPurchasedBook?> UpdateProgress(DTOUpdateProgress updateProgress, int userId)
        {
            var purchasedBook = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.BookId == updateProgress.BookId && pb.PurchaseStatus == PurchaseStatus.Success)
                .Include(pb => pb.PlayingPart).ThenInclude(bp => bp.Answers)
                .FirstOrDefaultAsync();

            if (purchasedBook == null) return null;

            if (updateProgress.PlayingPosition.HasValue)
            {
                purchasedBook.PlayingPosition = (decimal)updateProgress.PlayingPosition;
            }

            if (updateProgress.QuestionsActive.HasValue)
            {
                purchasedBook.QuestionsActive = (bool)updateProgress.QuestionsActive;
            }

            //if there is next book to be played set isBookPlaying to false to previous book and to true to next book
            if (updateProgress.NextBookId.HasValue)
            {
                purchasedBook.IsBookPlaying = false;

                var nextBook = await _dbContext.PurchasedBooks
                    .Where(pb => pb.UserId == userId && pb.BookId == updateProgress.NextBookId && pb.PurchaseStatus == PurchaseStatus.Success)
                    .FirstOrDefaultAsync();

                if (nextBook != null) nextBook.IsBookPlaying = true;
            }

            await _dbContext.SaveChangesAsync();

            var book = await GetBook(purchasedBook.BookId);
            var returnPurchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                PurchaseType = purchasedBook.PurchaseType,
                Language = purchasedBook.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(purchasedBook.PlayingPart),
                PlayingPosition = purchasedBook.PlayingPosition,
                IsBookPlaying = purchasedBook.IsBookPlaying,
                QuestionsActive = purchasedBook.QuestionsActive
            };

            return returnPurchasedBook;
        }

        public async Task<DTOReturnPurchasedBook?> StartBookAgain(int bookId, int userId)
        {
            var pb = await _dbContext.PurchasedBooks
                .Where(pb => pb.UserId == userId && pb.BookId == bookId && pb.PurchaseStatus == PurchaseStatus.Success)
                .FirstOrDefaultAsync();

            if (pb == null) return null;

            //set PlayingPart back to root part
            var rootPart = await _dbContext.BookParts
                .Where(bp => bp.BookId == pb.BookId && bp.IsRoot == true)
                .Include(bp=>bp.Answers)
                .FirstOrDefaultAsync();
            if (rootPart == null) return null;

            pb.PlayingPartId = rootPart.Id;
            pb.PlayingPosition = 0;
            pb.IsBookPlaying = false;
            await _dbContext.SaveChangesAsync();


            var book = await GetBook(pb.BookId);
            var purchasedBook = new DTOReturnPurchasedBook
            {
                Id = book.Id,
                Description = book.Description,
                Title = book.Title,
                ImageURL = book.ImageURL,
                PurchaseType = pb.PurchaseType,
                Language = pb.Language,
                PlayingPart = _mapper.Map<DTOReturnPart>(rootPart),
                PlayingPosition = pb.PlayingPosition,
                IsBookPlaying = pb.IsBookPlaying,
                QuestionsActive = pb.QuestionsActive
            };

            return purchasedBook;
        }


        #endregion

        #region DELETE

        public async Task<bool> RemoveCanceledPurchase(string sessionId)
        {
            try
            {
                // Find the purchase by sessionId
                var purchases = await _dbContext.PurchasedBooks
                                               .Where(pb => pb.SessionId == sessionId)
                                               .ToListAsync();

                if (purchases != null)
                {
                    // Remove the found purchase from the DbContext
                    _dbContext.PurchasedBooks.RemoveRange(purchases);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate that the removal was successful
                }
                else
                {
                    // No purchase found with the given sessionId
                    return false; // Indicate that no purchase was found to remove
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove a purchase: {ex.Message}");
                return false; // Indicate that an error occurred during the removal process
            }
        }

        public async Task RemoveBasketItem(BasketItem item)
        {
            _dbContext.BasketItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveBasketItems(int userId)
        {
            var basketItems = await _dbContext.BasketItems.Where(b => b.UserId == userId).ToListAsync();

            _dbContext.BasketItems.RemoveRange(basketItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveUserPendingPurchases(User user)
        {
            try
            {
                // Find the purchase by sessionId
                var purchases = await _dbContext.PurchasedBooks
                                               .Where(pb => pb.UserId == user.Id && pb.PurchaseStatus == PurchaseStatus.Pending)
                                               .ToListAsync();

                if (purchases != null)
                {
                    // Remove the found purchase from the DbContext
                    _dbContext.PurchasedBooks.RemoveRange(purchases);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                    return true; // Indicate that the removal was successful
                }
                else
                {
                    // No purchase found with the given sessionId
                    return false; // Indicate that no purchase was found to remove
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to remove a purchase: {ex.Message}");
                return false; // Indicate that an error occurred during the removal process
            }
        }

        #endregion
    }
}
