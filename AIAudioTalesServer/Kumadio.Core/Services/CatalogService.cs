using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Core.Interfaces;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;


namespace Kumadio.Core.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookPartRepository _bookPartRepository;

        public CatalogService(
            IUnitOfWork unitOfWork,
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository, 
            IBookPartRepository bookPartRepository
            )
        {
            _unitOfWork = unitOfWork;
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _bookPartRepository = bookPartRepository;
        }

        #region Books
        public async Task<Result<Book>> GetBook(int bookId)
        {
            var book = await _bookRepository.GetFirstWhere(b => b.Id == bookId);
            if (book == null) return DomainErrors.Catalog.BookNotFound;

            return book;
        }

        public async Task<Result<IList<Book>>> GetBooks(int categoryId, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _bookRepository.GetBooks(categoryId, skip, pageSize);
            if (books == null) return DomainErrors.Catalog.BooksNotFound;

            return Result<IList<Book>>.Success(books);
        }

        public async Task<Result<IList<Book>>> SearchBooks(string searchTerm, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var books = await _bookRepository.SearchBooks(searchTerm, skip, pageSize);
            if (books == null) return DomainErrors.Catalog.BooksNotFound;

            return Result<IList<Book>>.Success(books);
        }
        #endregion

        #region Parts
        public async Task<Result<PartTree>> GetPartTree(int bookId)
        {
            var rootPart = await _bookPartRepository.GetRootPartAsync(bookId);
            if (rootPart == null) return DomainErrors.Catalog.RootPartNotFound;

            // Build the tree
            var rootPartTree = new PartTree
            {
                PartId = rootPart.Id,
                PartName = "Intro",
                Answers = rootPart.Answers
            };

            rootPartTree.NextParts = await PopulateTree(rootPartTree.Answers);

            return rootPartTree;
        }

        private async Task<IList<PartTree>> PopulateTree(IList<Answer>? answers)
        {
            var partTreeList = new List<PartTree>();
            if (answers == null) return partTreeList;

            foreach (var answer in answers)
            {
                if (answer.NextPartId == null) continue;

                var childPart = await _bookPartRepository.GetFirstWhere(bp => bp.Id == answer.NextPartId.Value);
                if (childPart == null) continue;

                var newTree = new PartTree
                {
                    PartId = childPart.Id,
                    PartName = answer.Text,
                    Answers = childPart.Answers
                };

                newTree.NextParts = await PopulateTree(childPart.Answers);

                partTreeList.Add(newTree);
            }

            return partTreeList;
        }

        public async Task<Result<BookPart>> GetPart(int partId)
        {
            var part = await _bookPartRepository.GetFirstWhere(bp => bp.Id == partId);
            if (part == null) return DomainErrors.Catalog.BookPartNotFound;

            return part;
        }

        #endregion

        public async Task<Result<IList<Category>>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAll();

            if (categories == null) return DomainErrors.Catalog.CategoriesNotFound;

            return Result<IList<Category>>.Success(categories);
        }

    }
}
