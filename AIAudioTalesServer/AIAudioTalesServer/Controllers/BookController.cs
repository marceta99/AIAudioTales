using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public BookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("GetAllBooks")]
        public async Task<ActionResult<IList<BookReturnDTO>>> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("GetBooksFromCategory/{bookCategory}")]
        public async Task<ActionResult<IList<BookReturnDTO>>> GetBooksForCategory(BookCategory bookCategory)
        {
            var books = await _bookRepository.GetBooksForCategory(bookCategory);
            if (books == null)
            {
                return NotFound("There is no books for that category");
            }
            return Ok(books);
        }

        [HttpGet("GetUserBooks/{userId}")]
        public async Task<ActionResult<IList<BookReturnDTO>>> GetUserBooks(int userId)
        {
            var books = await _bookRepository.GetBooksFromSpecificUser(userId);
            if (books == null)
            {
                return NotFound("That user does not exists");
            }
            return Ok(books);

        }

        [HttpGet("GetBook/{bookId}")]
        public async Task<ActionResult<BookReturnDTO>> GetBook(int bookId)
        {
            var book = await _bookRepository.GetBook(bookId);
            if (book == null)
            {
                return NotFound("That book does not exists");
            }
            return Ok(book);
        }

        [HttpPost("AddNewBook")]
        public async Task<ActionResult<BookReturnDTO>> AddNewBook([FromBody] BookCreationDTO book)
        {
            if (ModelState.IsValid)
            {
                var newBook = await _bookRepository.AddNewBook(book);
                return CreatedAtAction(nameof(GetBook), new { bookId = newBook.Id }, newBook);
            }
            return BadRequest();
        }

        [HttpPost("PurchaseBook/{bookId}")]
        public ActionResult PurchaseBook(int bookId)
        {
            return NoContent();
        }

        [HttpPut("UpdateBookDetails/{bookId}")]
        public async Task<ActionResult> UpdateBookDetails(int bookId, [FromBody] BookUpdateDTO book)
        {
            if (!ModelState.IsValid || bookId != book.Id)
            {
                return BadRequest();
            }
            var result = await _bookRepository.UpdateBookDetails(book);
            if (result == 0)
            {
                return BadRequest("Problem with updating book details");
            }
            return NoContent();
        }

        [HttpDelete("DeleteBook/{bookId}")]
        public async Task<ActionResult> DeleteBook(int bookId)
        {
            var result = await _bookRepository.DeleteBook(bookId);
            if (result == 0)
            {
                return BadRequest("Problem with deleting book");
            }
            return NoContent();
        }

        [HttpPatch("UploadImageForBook/{bookId}")]
        public async Task<ActionResult> UploadImageForBook(int bookId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Invalid image file");
            }

            var result = await _bookRepository.UploadImageForBook(bookId, imageFile);
            if (result == 0)
            {
                return BadRequest("Problem with uploading the image");
            }
            return NoContent();

        }

        [HttpGet("GetBookImage/{bookId}")]
        public async Task<ActionResult<byte[]?>> GetBookImage(int bookId)
        {

            var bookImage = await _bookRepository.GetBookImage(bookId);
            if (bookImage == null)
            {
                return NotFound("That book does not exists");
            }
            return File(bookImage, "image/jpeg");
        }



    }
}
