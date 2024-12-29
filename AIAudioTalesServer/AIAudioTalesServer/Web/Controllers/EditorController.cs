using AIAudioTalesServer.Core.Interfaces;
using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Web.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        private readonly IEditorService _editorService;
        public EditorController(IEditorService editorService)
        {
            _editorService = editorService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var link = await _editorService.UploadAsync(file, Request);
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

                var result = await _editorService.AddRootPartAsync(root, Request);
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

                var result = await _editorService.AddBookPartAsync(dtoPart, Request);
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

            var newBookId = await _editorService.AddBookAsync(dto, user.Id);
            return Ok(newBookId);
        }
    }
}
