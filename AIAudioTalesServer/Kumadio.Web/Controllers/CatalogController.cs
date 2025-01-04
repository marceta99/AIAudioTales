using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<IList<Book>>> GetAllBooks()
        {
            var books = await _catalogService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IList<Category>>> GetAllCategories()
        {
            var cats = await _catalogService.GetAllCategoriesAsync();
            return Ok(cats);
        }

        [HttpGet("GetBook/{bookId}")]
        public async Task<ActionResult<DTOReturnBook>> GetBook(int bookId)
        {
            var dto = await _catalogService.GetBookAsync(bookId);
            if (dto == null) return NotFound("Book not found.");
            return Ok(dto);
        }

        [HttpGet("GetBooksFromCategory")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetBooksFromCategory(
            [FromQuery] int categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var dtos = await _catalogService.GetBooksFromCategoryAsync(categoryId, pageNumber, pageSize);
            return Ok(dtos);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IList<DTOReturnBook>>> SearchBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var books = await _catalogService.SearchBooksAsync(searchTerm, pageNumber, pageSize);
            return Ok(books);
        }

        [HttpGet("GetPart/{partId}")]
        public async Task<ActionResult<DTOReturnPart>> GetPart(int partId)
        {
            var result = await _catalogService.GetPartAsync(partId);
            if (result == null) return BadRequest("No part with that ID");
            return Ok(result);
        }

        [HttpGet("GetBookTree/{bookId}")]
        public async Task<ActionResult<DTOReturnTreePart>> GetBookTree(int bookId)
        {
            var tree = await _catalogService.GetBookTreeAsync(bookId);
            return Ok(tree);
        }
    }
}
