using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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

        #region GET

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
        public async Task<ActionResult<IList<DTOReturnBook>>> GetBooksForCategory([FromQuery] int bookCategory, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _booksRepository.GetBooksForCategory(bookCategory);
            if (books == null)
            {
                //return empty array if there is no books in that category
                return new List<DTOReturnBook>();
            }

            //skip elements until you came to that page that is specified in "page" and take only number elements from page that is specified in "pageSize"
            var paginatedBooks = books.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(paginatedBooks);
        }
        
        [HttpGet("UserHasBook/{bookId}")]
        public async Task<ActionResult<bool>> UserHasBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            bool hasBook = await _booksRepository.UserHasBook(bookId, user.Id);

            return Ok(hasBook);
        }

        [HttpGet("IsBasketItem/{bookId}")]
        public async Task<ActionResult<bool>> IsBasketItem(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            bool hasBook = await _booksRepository.IsBasketItem(bookId, user.Id);

            return Ok(hasBook);
        }

        [HttpGet("GetUserBooks")]
        public async Task<ActionResult<IList<DTOReturnPurchasedBook>>> GetUserBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var books = await _booksRepository.GetUserBooks(user.Id);

            return Ok(books);
        }

        [HttpGet("GetBasket")]
        public async Task<ActionResult<DTOBasket>> GetBasket()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var basket = await _booksRepository.GetBasket(user.Id);

            return Ok(basket);
        }

        [HttpGet("GetPurchasedBook/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetPurchasedBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var book = await _booksRepository.GetPurchasedBook(user.Id, bookId);

            if (book == null) return BadRequest("User does not have that book");

            return Ok(book);
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
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var history = await _booksRepository.GetSearchHistory(user.Id);

            return Ok(history);
        }

        #endregion

        #region POST
        /*[HttpPost("AddNewBook")]
        public async Task<ActionResult<DTOReturnBook>> AddNewBook([FromBody] DTOCreateBook book)
        {
            
            if (ModelState.IsValid)
            {
                var newBook = await _booksRepository.AddNewBook(book);
                return CreatedAtAction(nameof(GetBook), new { bookId = newBook.Id }, newBook);
            }
            return BadRequest();
        }*/

        [HttpPost("SaveSearchTerm")]
        public async Task<IActionResult> SaveSearchTerm([FromQuery] string searchTerm)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            await _booksRepository.SaveSearchTerm(user.Id, searchTerm);
            return Ok();
        }

        [HttpPost("AddBasketItem")]
        public async Task<ActionResult<DTOBasket>> AddBasketItem([FromQuery] int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _booksRepository.AddBasketItem(user.Id, bookId);
            if (result == null) return BadRequest();

            var newBasket = await _booksRepository.GetBasket(user.Id);

            return Ok(newBasket);
        }
        #endregion

        #region DELETE

        [HttpDelete("RemoveBasketItem")]
        public async Task<ActionResult<DTOBasket>> RemoveBasketItem([FromQuery] int itemId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _booksRepository.GetItemById(itemId);

            if (item == null) return BadRequest();

            await _booksRepository.RemoveBasketItem(item);

            var updatedBasket = await _booksRepository.GetBasket(user.Id);

            return updatedBasket;
        }

        #endregion
    }
}
