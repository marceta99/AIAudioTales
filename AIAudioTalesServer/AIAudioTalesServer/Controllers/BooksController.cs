using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using AIAudioTalesServer.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AIAudioTalesServer.Controllers
{
    [Authorize]
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

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IList<Category>>> GetAllCategories()
        {
            var categories = await _booksRepository.GetAllCategories();
            return Ok(categories);
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

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var userHasBook = await _booksRepository.UserHasBook(purchase.BookId, user.Id);
                    if (userHasBook) return BadRequest("User already has that book");

                    await _booksRepository.PurchaseBook(user.Id, purchase.BookId, purchase.PurchaseType, purchase.Language);

                    return Ok("Book was successfully purchased");
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("UserHasBook/{bookId}")]
        public async Task<ActionResult<bool>> UserHasBook(int bookId)
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    bool hasBook = await _booksRepository.UserHasBook(bookId, user.Id);

                    return Ok(hasBook);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("IsBasketItem/{bookId}")]
        public async Task<ActionResult<bool>> IsBasketItem(int bookId)
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    bool hasBook = await _booksRepository.IsBasketItem(bookId, user.Id);

                    return Ok(hasBook);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("GetUserBooks")]
        public async Task<ActionResult<IList<PurchasedBookReturnDTO>>> GetUserBooks()
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var books = await _booksRepository.GetUserBooks(user.Id);

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

        [HttpGet("GetBasket")]
        public async Task<ActionResult<BasketDTO>> GetBasket()
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var basket = await _booksRepository.GetBasket(user.Id);

                    return Ok(basket);
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

        [HttpGet("GetPurchasedBook/{bookId}")]
        public async Task<ActionResult<PurchasedBookReturnDTO>> GetPurchasedBook(int bookId)
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var book = await _booksRepository.GetPurchasedBook(user.Id, bookId);

                    if (book == null) return BadRequest("User does not have that book");

                    return Ok(book);
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


        [HttpGet("Search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {   
            var books = await _booksRepository.SearchBooks(searchTerm, pageNumber, pageSize);
            return Ok(books);
        }

        [HttpGet("GetSearchHistory")]
        public async Task<IActionResult> GetSearchHistory()
        {
            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var history = await _booksRepository.GetSearchHistory(user.Id);
                    return Ok(history);
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

        [HttpPost("SaveSearchTerm")]
        public async Task<IActionResult> SaveSearchTerm([FromQuery] string searchTerm)
        {

            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    await _booksRepository.SaveSearchTerm(user.Id, searchTerm);
                    return Ok();
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

        [HttpPost("AddBasketItem")]
        public async Task<ActionResult<BasketDTO>> AddBasketItem([FromQuery] int bookId)
        {

            // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var result = await _booksRepository.AddBasketItem(user.Id, bookId);
                    if (result == null) return BadRequest();

                    var newBasket = await _booksRepository.GetBasket(user.Id);

                    return Ok(newBasket);
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

        [HttpDelete("RemoveBasketItem")]
        public async Task<ActionResult<BasketDTO>> RemoveBasketItem([FromQuery] int itemId)
        {
            
             // Get the JWT token cookie
            var jwtTokenCookie = Request.Cookies["X-Access-Token"];

            if (!string.IsNullOrEmpty(jwtTokenCookie))
            {
                // Decode the JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtTokenCookie);

                // Access custom claim "email"
                var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email");

                if (emailClaim != null)
                {
                    var email = emailClaim.Value;

                    var user = await _authRepository.GetUserWithEmail(email);
                    if (user == null) return BadRequest();

                    var item = await _booksRepository.GetItemById(itemId);

                    if (item == null) return BadRequest();

                    await _booksRepository.RemoveBasketItem(item);

                    var updatedBasket = await _booksRepository.GetBasket(user.Id);

                    return updatedBasket;
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
    }
}
