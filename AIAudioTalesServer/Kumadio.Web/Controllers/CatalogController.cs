using Kumadio.Core.Interfaces;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;
using Kumadio.Web.Common;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IDtoMapper<Book, DTOReturnBook> _bookMapper;
        private readonly IDtoMapper<BookPart, DTOReturnPart> _partMapper;
        private readonly IDtoMapper<Category, DTOReturnCategory> _categoryMapper;
        private readonly IDtoMapper<PartTreeModel, DTOReturnPartTree> _partTreeMapper;

        public CatalogController(
            ICatalogService catalogService,
            IDtoMapper<Book, DTOReturnBook> bookMapper,
            IDtoMapper<BookPart, DTOReturnPart> partMapper,
            IDtoMapper<Category, DTOReturnCategory> categoryMapper,
            IDtoMapper<PartTreeModel, DTOReturnPartTree> partTreeMapper
            )
        {
            _catalogService = catalogService;
            _bookMapper = bookMapper;
            _partMapper = partMapper;
            _categoryMapper = categoryMapper;
            _partTreeMapper = partTreeMapper;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IList<DTOReturnCategory>>> GetAllCategories()
        {
            var categoriesResult = await _catalogService.GetAllCategories();
            if(categoriesResult.IsFailure) return categoriesResult.Error.ToBadRequest();

            return Ok(_categoryMapper.Map(categoriesResult.Value));
        }

        [HttpGet("books/{bookId}")]
        public async Task<ActionResult<DTOReturnBook>> GetBook(int bookId)
        {
            var bookResult = await _catalogService.GetBook(bookId);
            if(bookResult.IsFailure) return bookResult.Error.ToBadRequest();
            
            return Ok(_bookMapper.Map(bookResult.Value));
        }

        [HttpGet("books")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetBooks(
            [FromQuery] int categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var booksResult = await _catalogService.GetBooks(categoryId, pageNumber, pageSize);
            if (booksResult.IsFailure) return booksResult.Error.ToBadRequest();

            return Ok(_bookMapper.Map(booksResult.Value));
        }

        [HttpGet("search")]
        public async Task<ActionResult<IList<DTOReturnBook>>> SearchBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var booksResult = await _catalogService.SearchBooks(searchTerm, pageNumber, pageSize);
            if (booksResult.IsFailure) return booksResult.Error.ToBadRequest();

            return Ok(_bookMapper.Map(booksResult.Value));
        }

        [HttpGet("parts/{partId}")]
        public async Task<ActionResult<DTOReturnPart>> GetPart(int partId)
        {
            var partResult = await _catalogService.GetPart(partId);
            if (partResult.IsFailure) return partResult.Error.ToBadRequest();

            return Ok(_partMapper.Map(partResult.Value));
        }

        [HttpGet("part-tree/{bookId}")]
        public async Task<ActionResult<DTOReturnPartTree>> GetPartTree(int bookId)
        {
            var partTreeResult = await _catalogService.GetPartTree(bookId);
            if (partTreeResult.IsFailure) return partTreeResult.Error.ToBadRequest();

            return Ok(_partTreeMapper.Map(partTreeResult.Value));
        }
    }
}
