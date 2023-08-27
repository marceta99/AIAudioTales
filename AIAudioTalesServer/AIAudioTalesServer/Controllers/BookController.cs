using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AIAudioTalesServer.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthRepository _authRepository;
        public BookController(IBookRepository bookRepository, IAuthRepository authRepository)
        {
            _bookRepository = bookRepository;
            _authRepository = authRepository;
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

        [HttpGet("GetUserBooks")]
        public async Task<ActionResult<IList<BookReturnDTO>>> GetUserBooks()
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "userName"
                var userNameClaim = token.Claims.FirstOrDefault(c => c.Type == "userName");

                if (userNameClaim != null)
                {
                    var userName = userNameClaim.Value;

                    var user = await _authRepository.GetUserWithUserName(userName);
                    if (user == null) return BadRequest();

                    var books = await _bookRepository.GetBooksFromSpecificUser(user.Id);
                    
                    return Ok(books);    
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
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

        [HttpPost("PurchaseBook")]
        public async Task<ActionResult> PurchaseBook([FromBody] Purchase purchase)
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "userName"
                var userNameClaim = token.Claims.FirstOrDefault(c => c.Type == "userName");

                if (userNameClaim != null)
                {
                    var userName = userNameClaim.Value;

                    var user = await _authRepository.GetUserWithUserName(userName);
                    if (user == null) return BadRequest();

                    await _bookRepository.PurchaseBook(user.Id, purchase.BookId, purchase.PurchaseType, purchase.Language);

                    return Ok("Book was successfully purchased");
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound();
            }
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
