using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AIAudioTalesServer.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksRepository _booksRepository;
        public BooksController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
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

        [HttpGet("GetPurchasedBooks")]
        public async Task<ActionResult<IList<DTOReturnPurchasedBook>>> GetPurchasedBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var books = await _booksRepository.GetPurchasedBooks(user.Id);

            return Ok(books);
        }

        [HttpGet("GetCreatorBooks")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetCreatorBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var books = await _booksRepository.GetCreatorBooks(user.Id);

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

        [HttpGet("GetPart/{partId}")]
        public async Task<ActionResult<DTOReturnPart>> GetPart(int partId)
        {
            var bookPart = await _booksRepository.GetPart(partId);

            if (bookPart != null)
            {
                return Ok(bookPart);
            }
            return BadRequest("There is no part with that id");
        }

        [HttpGet("GetBookTree/{bookId}")]
        public async Task<ActionResult<DTOReturnTreePart>> GetBookTree(int bookId)
        {
            var treeParts = await _booksRepository.GetBookTree(bookId);

            return Ok(treeParts);
        }

        [HttpGet("GetCurrentBook")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetCurrentBook()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var currentBook = await _booksRepository.GetCurrentBook(user.Id);

            if (currentBook == null) return BadRequest("Problem with returning current book");

            return Ok(currentBook);
        }

        #endregion

        #region POST

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var audioLink = await _booksRepository.Upload(file, Request);
                return Ok(audioLink);
            }

            return BadRequest("Invalid file.");
        }

        [HttpPost("AddRootPart")]
        public async Task<ActionResult<DTOReturnPart?>> AddRootPart()
        {
            try
            {
                // Read the form data directly from the request
                var form = await Request.ReadFormAsync();

                // Read the form fields
                var partAudio = form.Files["partAudio"];
                var bookIdStr = form["bookId"].FirstOrDefault();
                var answersJson = form["answers"].FirstOrDefault();

                if (partAudio == null || string.IsNullOrEmpty(bookIdStr))
                {
                    return BadRequest("Audio file or book id is missing");
                }

                if (!int.TryParse(bookIdStr, out int bookId))
                {
                    return BadRequest("Invalid book id");
                }

                // Deserialize the answers
                var answers = JsonConvert.DeserializeObject<List<DTOCreateAnswer>>(answersJson);

                var newRoot = new DTOCreateRootPart
                {
                    PartAudio = partAudio,
                    BookId = bookId,
                    Answers = answers
                };

                var result = await _booksRepository.AddRootPart(newRoot, Request);

                if (result == null)
                {
                    return BadRequest("Book with that id does not exist");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("AddBookPart")]
        public async Task<ActionResult<DTOReturnPart?>> AddBookPart()
        {
            try
            {
                // Read the form data directly from the request
                var form = await Request.ReadFormAsync();

                // Read the form fields
                var partAudio = form.Files["partAudio"];
                var bookIdStr = form["bookId"].FirstOrDefault();
                var parentAnswerIdStr = form["parentAnswerId"].FirstOrDefault();
                var answersJson = form["answers"].FirstOrDefault();

                if (partAudio == null || string.IsNullOrEmpty(bookIdStr) || string.IsNullOrEmpty(parentAnswerIdStr))
                {
                    return BadRequest("Audio file or book is missing");
                }

                if (!int.TryParse(bookIdStr, out int bookId) || !int.TryParse(parentAnswerIdStr, out int parentAnswerId))
                {
                    return BadRequest("Invalid book or parentAnswer");
                }

                // Deserialize the answers
                var answers = JsonConvert.DeserializeObject<List<DTOCreateAnswer>>(answersJson);

                var newPart = new DTOCreatePart
                {
                    PartAudio = partAudio,
                    BookId = bookId,
                    Answers = answers,
                    ParentAnswerId = parentAnswerId
                };

                var result = await _booksRepository.AddBookPart(newPart, Request);

                if (result == null)
                {
                    return BadRequest("Bad input for book part creation, please try again");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddBook")]
        public async Task<ActionResult<int>> AddBook([FromBody] DTOCreateBook book)
        {

            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var newBook = await _booksRepository.AddBook(book, user.Id);
                //return CreatedAtAction(nameof(GetBook), new { bookId = newBook.Id }, newBook);
                return Ok(newBook.Id);
            }
            return BadRequest();
        }

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

        #region PATCH

        [HttpPatch("NextPart")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> NextPart(DTOUpdateNextPart nextPart)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var purchasedBook = await _booksRepository.NextPart(nextPart, user.Id);

            if (purchasedBook != null)
            {
                return Ok(purchasedBook);
            }
            return BadRequest("There is no part with that id");
        }

        [HttpPatch("ActivateQuestions/{bookId}")]
        public async Task<ActionResult> ActivateQuestions(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _booksRepository.ActivateQuestions(bookId, user.Id);

            if (result != 0)
            {
                return Ok();
            }
            return BadRequest("There was a problem with activating questions");
        }

        [HttpPatch("UpdateProgress")]
        public async Task<ActionResult> UpdateProgress(DTOUpdateProgress progress)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _booksRepository.UpdateProgress(progress, user.Id);

            if (result == false)
            {
                return BadRequest("There was a problem with updating progress");
            }
            return Ok();
        }

        [HttpPatch("StartBookAgain/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> StartBookAgain(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _booksRepository.StartBookAgain(bookId, user.Id);

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("There was a problem with updating progress");
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
