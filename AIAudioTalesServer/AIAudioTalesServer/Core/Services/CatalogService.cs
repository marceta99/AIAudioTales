using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Infrastructure.Interfaces;
using AIAudioTalesServer.Infrastructure.Repositories;
using AIAudioTalesServer.Web.DTOS;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace AIAudioTalesServer.Core.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;  
        public CatalogService(ICatalogRepository catalogRepository, IMapper mapper, IMemoryCache cache)
        {
            _catalogRepository = catalogRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<IList<Book>> GetAllBooksAsync()
        {
            // Example: place caching here instead of repository
            const string cacheKey = "all_books";
            if (!_cache.TryGetValue(cacheKey, out IList<Book>? books))
            {
                books = await _catalogRepository.GetAllBooksAsync();
                _cache.Set(cacheKey, books, TimeSpan.FromMinutes(5));
            }
            return books ?? new List<Book>();
        }

        public async Task<IList<Category>> GetAllCategoriesAsync()
        {
            const string cacheKey = "all_categories";
            if (!_cache.TryGetValue(cacheKey, out IList<Category>? categories))
            {
                categories = await _catalogRepository.GetAllCategoriesAsync();
                _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(5));
            }
            return categories ?? new List<Category>();
        }

        public async Task<DTOReturnBook?> GetBookAsync(int bookId)
        {
            var book = await _catalogRepository.GetBookByIdAsync(bookId);
            if (book == null) return null;
            return _mapper.Map<DTOReturnBook>(book);
        }


        public async Task<IList<DTOReturnBook>> GetBooksFromCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _catalogRepository.GetBooksByCategoryAsync(categoryId, skip, pageSize);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }

        public async Task<DTOReturnTreePart?> GetBookTreeAsync(int bookId)
        {
            var rootPart = await _catalogRepository.GetRootPartAsync(bookId);
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

        public async Task<BookPart?> GetRootPart(int bookId)
        {
            return await _catalogRepository.GetRootPartAsync(bookId);
        }

        private async Task<IList<DTOReturnTreePart>> PopulateTree(IList<Answer>? answers)
        {
            if (answers == null) return new List<DTOReturnTreePart>();

            var list = new List<DTOReturnTreePart>();
            foreach (var ans in answers)
            {
                if (ans.NextPartId == null) continue;
                var childPart = await _catalogRepository.GetBookPartAsync(ans.NextPartId.Value);
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

        public async Task<DTOReturnPart?> GetPartAsync(int partId)
        {
            var part = await _catalogRepository.GetBookPartAsync(partId);
            if (part == null) return null;
            return _mapper.Map<DTOReturnPart>(part);
        }

        public async Task<IList<DTOReturnBook>> SearchBooksAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _catalogRepository.SearchBooksAsync(searchTerm, skip, pageSize);
            return _mapper.Map<IList<DTOReturnBook>>(books);
        }
    }
}
