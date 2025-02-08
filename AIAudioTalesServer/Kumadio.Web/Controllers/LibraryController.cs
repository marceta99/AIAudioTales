using Kumadio.Core.Interfaces;
using Kumadio.Domain.Entities;
using Kumadio.Web.Common;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using Kumadio.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IDtoMapper<PurchasedBook, DTOReturnPurchasedBook> _pbMapper;
        private readonly IDtoMapper<Book, DTOReturnBook> _bookMapper;
        private readonly OpenAISettings _openAISettings;
        public LibraryController(
            ILibraryService libraryService,
            IDtoMapper<PurchasedBook, DTOReturnPurchasedBook> pbMapper,
            IDtoMapper<Book, DTOReturnBook> bookMapper,
            IOptions<OpenAISettings> openAISettings)
        {
            _libraryService = libraryService;
            _pbMapper = pbMapper;
            _bookMapper = bookMapper;
            _openAISettings = openAISettings.Value;
        }

        #region GET

        [HttpGet("user-has-book/{bookId}")]
        public async Task<ActionResult<bool>> UserHasBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var hasBookResult = await _libraryService.UserHasBook(bookId, user.Id);
            if (hasBookResult.IsFailure) return hasBookResult.Error.ToBadRequest();

            return Ok(hasBookResult.Value);
        }


        [HttpGet("purchased-books")]
        public async Task<ActionResult<IList<DTOReturnPurchasedBook>>> GetPurchasedBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var pbResult = await _libraryService.GetPurchasedBooks(user.Id);
            if (pbResult.IsFailure) return pbResult.Error.ToBadRequest();
            
            return Ok(_pbMapper.Map(pbResult.Value));
        }

        [HttpGet("creator/books")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetCreatorBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var booksResult = await _libraryService.GetCreatorBooks(user.Id);
            if (booksResult.IsFailure) return booksResult.Error.ToBadRequest();

            return Ok(_bookMapper.Map(booksResult.Value));
        }

        [HttpGet("purchased-book/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetPurchasedBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var pbResult = await _libraryService.GetPurchasedBook(user.Id, bookId);
            if (pbResult.IsFailure) return pbResult.Error.ToBadRequest();
            
            return Ok(_pbMapper.Map(pbResult.Value));
        }



        [HttpGet("search-history")]
        public async Task<ActionResult<IList<string>>> GetSearchHistory()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var historyResult = await _libraryService.GetSearchHistory(user.Id);
            if (historyResult.IsFailure) return historyResult.Error.ToBadRequest();

            return Ok(historyResult.Value);
        }

        [HttpGet("current-book")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetCurrentBook()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var currentResult = await _libraryService.GetCurrentBook(user.Id);
            if (currentResult.IsFailure) return currentResult.Error.ToBadRequest();

            return Ok(_pbMapper.Map(currentResult.Value));
        }

        #endregion

        #region POST

        [HttpPost("search-term")]
        public async Task<IActionResult> AddSearchTerm([FromQuery] string searchTerm)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var addTermResult =  await _libraryService.AddSearchTerm(user.Id, searchTerm);
            if (addTermResult.IsFailure) return addTermResult.Error.ToBadRequest(); 

            return Ok();
        }

        [HttpPost("process-response")]
        public async Task<IActionResult> ProcessResponse([FromBody] DTOUserResponse dto)
        {
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

        [HttpPost("library/{bookId}")]
        public async Task<IActionResult> AddToLibrary(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var libraryResult = await _libraryService.AddToLibrary(user, bookId);
            if (libraryResult.IsFailure) return libraryResult.Error.ToBadRequest();

            return Ok();
        }

        [HttpPatch("next-part")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> NextPart([FromBody] DTOUpdateNextPart nextPart)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var nextPartResult = await _libraryService.NextPart(nextPart.BookId, nextPart.NextPartId, user.Id);
            if (nextPartResult.IsFailure) return nextPartResult.Error.ToBadRequest();

            return Ok(_pbMapper.Map(nextPartResult.Value));
        }

        [HttpPatch("ActivateQuestions")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> ActivateQuestions(
            [FromBody] DTOUpdateActivateQuestions activate)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.ActivateQuestionsAsync(
                activate.BookId, user.Id, activate.PlayingPosition);

            if (result == null) return BadRequest("Cannot activate questions");
            return Ok(result);
        }

        [HttpPatch("UpdateProgress")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> UpdateProgress(DTOUpdateProgress progress)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.UpdateProgressAsync(progress, user.Id);
            if (result == null) return BadRequest("Problem updating progress");
            return Ok(result);
        }

        [HttpPatch("StartBookAgain/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> StartBookAgain(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.StartBookAgainAsync(bookId, user.Id);
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

            var updatedBasket = await _libraryService.RemoveBasketItemAsync(user.Id, itemId);
            return updatedBasket;
        }

        #endregion
    }
}
