using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
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

        [HttpPost("AddNewBook")]
        public async Task<ActionResult<BookReturnDTO>> AddNewBook([FromForm] BookCreateDTO book)
        {
            
            if (book.ImageFile == null || book.ImageFile.Length == 0)
            {
                return BadRequest("Invalid image file");
            }
            if (ModelState.IsValid)
            {
                var newBook = await _booksRepository.AddNewBook(book);
                return CreatedAtAction(nameof(GetBook), new { bookId = newBook.Id }, newBook);
            }
            return BadRequest();
        }
       
    }
}
