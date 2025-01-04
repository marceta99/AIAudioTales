using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Settings;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.AspNetCore.Http;
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
        private readonly OpenAISettings _openAISettings;
        public LibraryController(ILibraryService libraryService, IOptions<OpenAISettings> openAISettings)
        {
            _libraryService = libraryService;
            _openAISettings = openAISettings.Value;
        }

        #region GET

        [HttpGet("UserHasBook/{bookId}")]
        public async Task<ActionResult<bool>> UserHasBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            bool hasBook = await _libraryService.UserHasBookAsync(bookId, user.Id);
            return Ok(hasBook);
        }

        [HttpGet("IsBasketItem/{bookId}")]
        public async Task<ActionResult<bool>> IsBasketItem(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            bool isBasket = await _libraryService.IsBasketItemAsync(bookId, user.Id);
            return Ok(isBasket);
        }

        [HttpGet("GetPurchasedBooks")]
        public async Task<ActionResult<IList<DTOReturnPurchasedBook>>> GetPurchasedBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.GetPurchasedBooksAsync(user.Id);
            return Ok(result);
        }

        [HttpGet("GetCreatorBooks")]
        public async Task<ActionResult<IList<DTOReturnBook>>> GetCreatorBooks()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.GetCreatorBooksAsync(user.Id);
            return Ok(result);
        }

        [HttpGet("GetBasket")]
        public async Task<ActionResult<DTOBasket>> GetBasket()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var basket = await _libraryService.GetBasketAsync(user.Id);
            return Ok(basket);
        }

        [HttpGet("GetPurchasedBook/{bookId}")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetPurchasedBook(int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var purchased = await _libraryService.GetPurchasedBookAsync(user.Id, bookId);
            if (purchased == null) return BadRequest("User does not have that book.");
            return Ok(purchased);
        }



        [HttpGet("GetSearchHistory")]
        public async Task<ActionResult<IList<string>>> GetSearchHistory()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var history = await _libraryService.GetSearchHistoryAsync(user.Id);
            return Ok(history);
        }



        [HttpGet("GetCurrentBook")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> GetCurrentBook()
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var current = await _libraryService.GetCurrentBookAsync(user.Id);
            if (current == null) return BadRequest("No current book for this user");
            return Ok(current);
        }

        #endregion

        #region POST

        [HttpPost("SaveSearchTerm")]
        public async Task<IActionResult> SaveSearchTerm([FromQuery] string searchTerm)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            await _libraryService.SaveSearchTermAsync(user.Id, searchTerm);
            return Ok();
        }

        [HttpPost("AddBasketItem")]
        public async Task<ActionResult<DTOBasket>> AddBasketItem([FromQuery] int bookId)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var basket = await _libraryService.AddBasketItemAsync(user.Id, bookId);
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

            var success = await _libraryService.AddToLibraryAsync(user, bookId);
            if (!success) return BadRequest("Could not add book to library");
            return Ok();
        }

        [HttpPatch("NextPart")]
        public async Task<ActionResult<DTOReturnPurchasedBook>> NextPart([FromBody] DTOUpdateNextPart nextPart)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var result = await _libraryService.NextPartAsync(nextPart, user.Id);
            if (result == null) return BadRequest("No part or purchase found");
            return Ok(result);
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
