using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Data.Repositories;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IAuthorizationService _authorizationService;
        public StoryController(IStoryRepository storyRepository, IAuthorizationService authorizationService)
        {
            _storyRepository = storyRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpPost("AddNewStoryToBook/{bookId}")]
        public async Task<ActionResult<StoryReturnDTO>> AddNewStoryToBook(int bookId, [FromBody] StoryCreationDTO story)
        {
            if (ModelState.IsValid)
            {
                var newStory = await _storyRepository.AddNewStoryToBook(bookId, story);
                
                if (newStory == null) return BadRequest("Book with that id does not exists");

                return CreatedAtAction(nameof(GetStory), new { storyId = newStory.Id }, newStory);
            }
            return BadRequest();
        }
        [HttpGet("GetStory/{storyId}")]
        public async Task<ActionResult<StoryReturnDTO>> GetStory(int storyId)
        {
            var book = await _storyRepository.GetStory(storyId);
            if (book == null)
            {
                return NotFound("That story does not exists");
            }
            return Ok(book);
        }

        [HttpDelete("DeleteStory/{storyId}")]
        public async Task<ActionResult> DeleteStory(int storyId)
        {
            var result = await _storyRepository.DeleteStory(storyId);
            if (result == 0)
            {
                return BadRequest("Problem with deleting story");
            }
            return NoContent();
        }

        [HttpGet("GetStoryAudio/{storyId}")]
        public async Task<ActionResult<byte[]?>> GetStoryAudio(int storyId)
        {
            var storyAudio = await _storyRepository.GetStoryAudio(storyId);
            if (storyAudio == null)
            {
                return NotFound("That story does not exists");
            }
            return File(storyAudio, "audio/mp3");
        }

        [HttpGet("GetStoriesForBook/{bookId}")]
        public async Task<ActionResult<IList<StoryReturnDTO>>> GetStoriesForBook(int bookId)
        {
            var stories = await _storyRepository.GetAllStoriesForBook(bookId);
            if (stories == null)
            {
                return NotFound("That book does not exists");
            }
            return Ok(stories);
        }

        [Authorize(Policy ="ListenerOnly")]
        [HttpGet("GetPlayableStoriesForBook/{bookId}")]
        public async Task<ActionResult<IList<StoryReturnDTO>>> GetPlayableStoriesForBook(int bookId)
        {
            var stories = await _storyRepository.GetAllPlayableStoriesForBook(bookId);
            if (stories == null)
            {
                return NotFound("That book does not exists");
            }
            return Ok(stories);
        }


        [HttpPut("UpdateStoryDetails/{storyId}")]
        public async Task<ActionResult> UpdateStoryDetails(int storyId, [FromBody] StoryUpdateDTO story)
        {
            if (!ModelState.IsValid || storyId != story.Id)
            {
                return BadRequest();
            }
            var result = await _storyRepository.UpdateStoryDetails(story);
            if (result == 0)
            {
                return BadRequest("Problem with updating story details");
            }
            return NoContent();
        }

        [HttpPatch("UploadAudioForStory/{storyId}")]
        public async Task<IActionResult> UploadAudioFile(int storyId, IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
            {
                return BadRequest("Invalid audio file");
            }

            var result = await _storyRepository.UploadAudioForStory(storyId, audioFile);
            if (result == 0)
            {
                return BadRequest("Problem with uploading the audio");
            }
            return NoContent();
        }
    }
}
