using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthRepository _authRepository;
        public BooksController(IBooksRepository booksRepository, IAuthRepository authRepository)
        {
            _booksRepository = booksRepository;
            _authRepository = authRepository;
        }

        [HttpGet("Test")]
        public ActionResult<string> Test()
        {
            return "Test is it everything working";
        }

        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<IList<Book>>> GetAllBooks()
        {
            var books = await _booksRepository.GetAllBooks();
            return Ok(books);
        }
        [HttpGet("GetBook/{bookId}")]
        public async Task<ActionResult<Book>> GetBook(int bookId)
        {
            var book = await _booksRepository.GetBook(bookId);
            if (book == null)
            {
                return NotFound("That book does not exists");
            }
            return Ok(book);
        }

        [HttpGet("GetBooksFromCategory")]
        public async Task<ActionResult<IList<BookReturnDTO>>> GetBooksForCategory([FromQuery] BookCategory bookCategory, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _booksRepository.GetBooksForCategory(bookCategory);
            if (books == null)
            {
                //return empty array if there is no books in that category
                return new List<BookReturnDTO>();
            }

            //skip elements until you came to that page that is specified in "page" and take only number elements from page that is specified in "pageSize"
            var paginatedBooks = books.Skip((page - 1)*pageSize).Take(pageSize);
            return Ok(paginatedBooks);
        }

        [HttpPost("AddNewBook")]
        public async Task<ActionResult<BookReturnDTO>> AddNewBook([FromBody] BookCreateDTO book)
        {
            
            if (ModelState.IsValid)
            {
                var newBook = await _booksRepository.AddNewBook(book);
                return CreatedAtAction(nameof(GetBook), new { bookId = newBook.Id }, newBook);
            }
            return BadRequest();
        }
       
    }
}
