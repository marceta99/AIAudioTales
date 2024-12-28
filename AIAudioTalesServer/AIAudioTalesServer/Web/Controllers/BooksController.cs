// FILE: BooksController.cs
using AIAudioTalesServer.Application.Services;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly OpenAISettings _openAISettings;

        public BooksController(
            IBooksService booksService,
            IOptions<OpenAISettings> openAISettings)
        {
            _booksService = booksService;
            _openAISettings = openAISettings.Value;
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
            var books = await _booksService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IList<Category>>> GetAllCategories()
        {
            var cats = await _booksService.GetAllCategoriesAsync();
            return Ok(cats);
        }

        [HttpGet("GetBook/{bookId}")]
        public async Task<ActionResult<DTOReturnBook>> GetBook(int bookId)
        {
            var dto = await _booksService.GetBookAsync(bookId);
            if (dto == null) return NotFound("Book not found.");
            return Ok(dto);
        }

        [HttpGet("GetBooksFromCategory")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetBooksFromCategory(
            [FromQuery] int categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var dtos = await _booksService.GetBooksFromCategoryAsync(categoryId, pageNumber, pageSize);
            return Ok(dtos);
        }

        [HttpGet("UserHasBook/{bookId}")]
        public async Task<ActionResult<bool>> UserHasBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            bool hasBook = await _booksService.UserHasBookAsync(bookId, user.Id);
            return Ok(hasBook);
        }

        [HttpGet("IsBasketItem/{bookId}")]
        public async Task<ActionResult<bool>> IsBasketItem(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            bool isBasket = await _booksService.IsBasketItemAsync(bookId, user.Id);
            return Ok(isBasket);
        }

        [HttpGet("GetPurchasedBooks")]
        public async Task<ActionResult<IList<DTOReturnPurchasedBook>>> GetPurchasedBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.GetPurchasedBooksAsync(user.Id);
            return Ok(result);
        }

        [HttpGet("GetCreatorBooks")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetCreatorBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.GetCreatorBooksAsync(user.Id);
            return Ok(result);
        }

        [HttpGet("GetBasket")]
        public async Task<ActionResult<DTOBasket>> GetBasket()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var basket = await _booksService.GetBasketAsync(user.Id);
            return Ok(basket);
        }

        [HttpGet("GetPurchasedBook/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetPurchasedBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var purchased = await _booksService.GetPurchasedBookAsync(user.Id, bookId);
            if (purchased == null) return BadRequest("User does not have that book.");
            return Ok(purchased);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IList<DTOReturnBook>>> SearchBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var books = await _booksService.SearchBooksAsync(searchTerm, pageNumber, pageSize);
            return Ok(books);
        }

        [HttpGet("GetSearchHistory")]
        public async Task<ActionResult<IList<string>>> GetSearchHistory()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var history = await _booksService.GetSearchHistoryAsync(user.Id);
            return Ok(history);
        }

        [HttpGet("GetPart/{partId}")]
        public async Task<ActionResult<DTOReturnPart>> GetPart(int partId)
        {
            var result = await _booksService.GetPartAsync(partId);
            if (result == null) return BadRequest("No part with that ID");
            return Ok(result);
        }

        [HttpGet("GetBookTree/{bookId}")]
        public async Task<ActionResult<DTOReturnTreePart>> GetBookTree(int bookId)
        {
            var tree = await _booksService.GetBookTreeAsync(bookId);
            return Ok(tree);
        }

        [HttpGet("GetCurrentBook")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetCurrentBook()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var current = await _booksService.GetCurrentBookAsync(user.Id);
            if (current == null) return BadRequest("No current book for this user");
            return Ok(current);
        }

        #endregion

        #region POST

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var link = await _booksService.UploadAsync(file, Request);
            if (string.IsNullOrEmpty(link)) return BadRequest("Invalid file");
            return Ok(link);
        }

        [HttpPost("AddRootPart")]
        public async Task<ActionResult<DTOReturnPart?>> AddRootPart()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var partAudio = form.Files["partAudio"];
                var bookIdStr = form["bookId"].FirstOrDefault();
                var answersJson = form["answers"].FirstOrDefault();

                if (partAudio == null || string.IsNullOrEmpty(bookIdStr))
                    return BadRequest("Missing audio file or bookId");

                if (!int.TryParse(bookIdStr, out int bookId))
                    return BadRequest("Invalid bookId");

                var answers = string.IsNullOrEmpty(answersJson)
                    ? null
                    : JsonConvert.DeserializeObject<List<DTOCreateAnswer>>(answersJson);

                var root = new DTOCreateRootPart
                {
                    BookId = bookId,
                    PartAudio = partAudio,
                    Answers = answers
                };

                var result = await _booksService.AddRootPartAsync(root, Request);
                if (result == null) return BadRequest("Failed to add root part.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddBookPart")]
        public async Task<ActionResult<DTOReturnPart?>> AddBookPart()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var partAudio = form.Files["partAudio"];
                var bookIdStr = form["bookId"].FirstOrDefault();
                var parentAnswerIdStr = form["parentAnswerId"].FirstOrDefault();
                var answersJson = form["answers"].FirstOrDefault();

                if (partAudio == null ||
                    string.IsNullOrEmpty(bookIdStr) ||
                    string.IsNullOrEmpty(parentAnswerIdStr))
                {
                    return BadRequest("Missing partAudio, bookId, or parentAnswerId");
                }

                if (!int.TryParse(bookIdStr, out int bookId) ||
                    !int.TryParse(parentAnswerIdStr, out int parentAnswerId))
                {
                    return BadRequest("Invalid Ids");
                }

                var answers = string.IsNullOrEmpty(answersJson)
                    ? null
                    : JsonConvert.DeserializeObject<List<DTOCreateAnswer>>(answersJson);

                var dtoPart = new DTOCreatePart
                {
                    BookId = bookId,
                    ParentAnswerId = parentAnswerId,
                    PartAudio = partAudio,
                    Answers = answers
                };

                var result = await _booksService.AddBookPartAsync(dtoPart, Request);
                if (result == null) return BadRequest("Failed to add book part");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddBook")]
        public async Task<ActionResult<int>> AddBook([FromBody] DTOCreateBook dto)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest();

            var newBookId = await _booksService.AddBookAsync(dto, user.Id);
            return Ok(newBookId);
        }

        [HttpPost("SaveSearchTerm")]
        public async Task<IActionResult> SaveSearchTerm([FromQuery] string searchTerm)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            await _booksService.SaveSearchTermAsync(user.Id, searchTerm);
            return Ok();
        }

        [HttpPost("AddBasketItem")]
        public async Task<ActionResult<DTOBasket>> AddBasketItem([FromQuery] int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var basket = await _booksService.AddBasketItemAsync(user.Id, bookId);
            return Ok(basket);
        }

        [HttpPost("ProcessChildResponse")]
        public async Task<IActionResult> ProcessChildResponse([FromBody] DTOChildResponse dto)
        {
            // Example using OpenAI. 
            var apiKey = _openAISettings.ApiKey;
            var apiUrl = _openAISettings.ApiUrl;
            var model = _openAISettings.Model;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model,
                messages = new[] { new { role = "user", content = dto.Prompt } },
                max_tokens = 10,
                temperature = 0.3
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );
            try
            {
                var response = await client.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, responseString);

                var data = JsonConvert.DeserializeObject<dynamic>(responseString);
                var assistantReply = data.choices[0].message.content.ToString().Trim();
                return Ok(new { reply = assistantReply });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region PATCH

        [HttpPost("AddToLibrary/{bookId}")]
        public async Task<IActionResult> AddToLibrary(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var success = await _booksService.AddToLibraryAsync(user, bookId);
            if (!success) return BadRequest("Could not add book to library");
            return Ok();
        }

        [HttpPatch("NextPart")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> NextPart([FromBody] DTOUpdateNextPart nextPart)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.NextPartAsync(nextPart, user.Id);
            if (result == null) return BadRequest("No part or purchase found");
            return Ok(result);
        }

        [HttpPatch("ActivateQuestions")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> ActivateQuestions(
            [FromBody] DTOUpdateActivateQuestions activate)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.ActivateQuestionsAsync(
                activate.BookId, user.Id, activate.PlayingPosition);

            if (result == null) return BadRequest("Cannot activate questions");
            return Ok(result);
        }

        [HttpPatch("UpdateProgress")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> UpdateProgress(DTOUpdateProgress progress)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.UpdateProgressAsync(progress, user.Id);
            if (result == null) return BadRequest("Problem updating progress");
            return Ok(result);
        }

        [HttpPatch("StartBookAgain/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> StartBookAgain(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _booksService.StartBookAgainAsync(bookId, user.Id);
            if (result == null) return BadRequest("Problem resetting book");
            return Ok(result);
        }

        #endregion

        #region DELETE

        [HttpDelete("RemoveBasketItem")]
        public async Task<ActionResult<DTOBasket>> RemoveBasketItem([FromQuery] int itemId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var updatedBasket = await _booksService.RemoveBasketItemAsync(user.Id, itemId);
            return updatedBasket;
        }

        #endregion
    }
}
