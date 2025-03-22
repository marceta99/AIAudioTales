using Kumadio.Core.Interfaces;
using Kumadio.Core.Models;
using Kumadio.Domain.Entities;
using Kumadio.Web.Common;
using Kumadio.Web.DTOS;
using Kumadio.Web.Mappers.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kumadio.Web.Controllers
{
    [Route("api/editor")]
    [ApiController]
    public class EditorController : ControllerBase
    {
        private readonly IEditorService _editorService;
        private readonly IDtoMapper<BookPart, DTOReturnPart> _partMapper;
        private readonly IDtoMapper<Book, DTOReturnBook> _bookMapper;

        public EditorController(IEditorService editorService, IDtoMapper<BookPart, DTOReturnPart> partMapper, IDtoMapper<Book, DTOReturnBook> bookMapper)
        {
            _editorService = editorService;
            _partMapper = partMapper;
            _bookMapper = bookMapper;
        }

        [HttpPost("root-part")]
        public async Task<ActionResult<DTOReturnPart>> AddRootPart()
        {
            var form = await Request.ReadFormAsync();
            var partAudio = form.Files["partAudio"];
            var bookIdStr = form["bookId"].FirstOrDefault();
            var answersJson = form["answers"].FirstOrDefault();

            if (partAudio == null || partAudio.Length == 0 || string.IsNullOrEmpty(bookIdStr))
                return BadRequest("Missing audio file or bookId");

            if (!int.TryParse(bookIdStr, out int bookId))
                return BadRequest("Invalid bookId for root part");

            var answers = string.IsNullOrEmpty(answersJson)
                ? null
                : JsonConvert.DeserializeObject<List<string>>(answersJson);

            if (answers == null) 
                return BadRequest("Invalid answers");

            // convert IFormFile -> byte[]
            byte[] audioBytes;
            using (var ms = new MemoryStream())
            {
                await partAudio.CopyToAsync(ms);
                audioBytes = ms.ToArray();
            }

            var extension = Path.GetExtension(partAudio.FileName);

            var rootPart = new RootPartModel
            {
                BookId = bookId,
                AudioBytes = audioBytes,
                Extension = extension,
                AnswersText = answers
            };

            var scheme = Request.Scheme; // "http" or "https"
            var hostValue = Request.Host.Value;
            var baseUrl = $"{scheme}://{hostValue}";

            var rootResult = await _editorService.AddRootPart(rootPart, baseUrl);
            if (rootResult.IsFailure) return rootResult.Error.ToBadRequest();

            return Ok(_partMapper.Map(rootResult.Value));
        }

        [HttpPost("part")]
        public async Task<ActionResult<DTOReturnPart>> AddBookPart()
        {
            var form = await Request.ReadFormAsync();
            var partAudio = form.Files["partAudio"];
            var bookIdStr = form["bookId"].FirstOrDefault();
            var parentAnswerIdStr = form["parentAnswerId"].FirstOrDefault();
            var answersJson = form["answers"].FirstOrDefault();

            if (partAudio == null
                || partAudio.Length == 0
                || string.IsNullOrEmpty(bookIdStr) 
                || string.IsNullOrEmpty(parentAnswerIdStr))
                return BadRequest("Missing partAudio, bookId, or parentAnswerId");
                

            if (!int.TryParse(bookIdStr, out int bookId) || !int.TryParse(parentAnswerIdStr, out int parentAnswerId))
                return BadRequest("Invalid Ids");
                

            var answers = string.IsNullOrEmpty(answersJson)
                ? null
                : JsonConvert.DeserializeObject<List<string>>(answersJson);

            if (answers == null)
                return BadRequest("Invalid answers");

            // convert IFormFile -> byte[]
            byte[] audioBytes;
            using (var ms = new MemoryStream())
            {
                await partAudio.CopyToAsync(ms);
                audioBytes = ms.ToArray();
            }

            var extension = Path.GetExtension(partAudio.FileName);

            var part = new PartModel
            {
                BookId = bookId,
                ParentAnswerId = parentAnswerId,
                AudioBytes = audioBytes,
                Extension = extension,
                AnswersText = answers
            };

            var scheme = Request.Scheme; // "http" or "https"
            var hostValue = Request.Host.Value;
            var baseUrl = $"{scheme}://{hostValue}";

            var partResult = await _editorService.AddBookPart(part, baseUrl);
            if (partResult.IsFailure) return partResult.Error.ToBadRequest();

            return Ok(_partMapper.Map(partResult.Value));
        }

        [HttpPost("book")]
        public async Task<ActionResult<DTOReturnBook>> AddBook([FromBody] DTOCreateBook bookDto)
        {
            var user = HttpContext.Items["CurrentUser"] as User;
            if (user == null) return Unauthorized();

            var book = new Book
            {
                CategoryId = bookDto.CategoryId,
                Description = bookDto.Description,
                ImageURL = bookDto.ImageURL,
                Price = bookDto.Price,
                Title = bookDto.Title,
                CreatorId = user.Id
            };

            var bookResult = await _editorService.AddBook(book);
            if (bookResult.IsFailure) return bookResult.Error.ToBadRequest();
            
            return Ok(_bookMapper.Map(bookResult.Value));
        }
    }
}
