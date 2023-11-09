using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Data.Repositories
{
    public class StoryRepository
    {   /*
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public StoryRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<StoryReturnDTO?> AddNewStoryToBook(int bookId, StoryCreationDTO newStory)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();
            if (book == null) return null; 

            var story = _mapper.Map<Story>(newStory);
            var createdStory = _dbContext.Stories.Add(story);
            await _dbContext.SaveChangesAsync();

            var returnStory = _mapper.Map<StoryReturnDTO>(createdStory.Entity);
            return returnStory;
        }

        public async Task<int> DeleteStory(int storyId)
        {
            var storyToDelete = await _dbContext.Stories.Where(s => s.Id == storyId).FirstOrDefaultAsync();
            if (storyToDelete == null)
            {
                return 0;
            }
            _dbContext.Stories.Remove(storyToDelete);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<StoryReturnDTO>> GetAllStoriesForBook(int bookId)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return null;

            var stories = await _dbContext.Stories.Where(s => s.BookId== book.Id).ToListAsync();

            var returnStories = _mapper.Map<IList<StoryReturnDTO>>(stories);
            return returnStories ;
        }

        public async Task<IList<StoryReturnDTO>> GetAllPlayableStoriesForBook(int bookId)
        {
            var book = await _dbContext.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();

            if (book == null) return null;

            var stories = await _dbContext.Stories.Where(s => s.BookId == book.Id).ToListAsync();
            var returnStories = _mapper.Map<IList<StoryReturnDTO>>(stories);
            
            return returnStories;
        }

        public async Task<StoryReturnDTO> GetStory(int storyId)
        {
            var story = await _dbContext.Stories.Where(s => s.Id == storyId).FirstOrDefaultAsync();

            var returnStory = _mapper.Map<StoryReturnDTO>(story);
            return returnStory;
        }

        public async Task<byte[]?> GetStoryAudio(int storyId)
        {
            var story = await _dbContext.Stories.Where(s => s.Id == storyId).FirstOrDefaultAsync();

            if (story == null) return null;

            return story.AudioData;
        }

        public async Task<int> UpdateStoryDetails(StoryUpdateDTO story)
        {
            var storyToEdit = await _dbContext.Stories.Where(s => s.Id == story.Id).FirstOrDefaultAsync();
            if (storyToEdit == null)
            {
                return 0;
            }
            storyToEdit.Text = story.Text;
            storyToEdit.Title = story.Title;

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UploadAudioForStory(int storyId, IFormFile audioFile)
        {
            var storyToEdit = await _dbContext.Stories.Where(b => b.Id == storyId).FirstOrDefaultAsync();
            if (storyToEdit == null)
            {
                return 0;
            }

            using (var memoryStream = new MemoryStream())
            {
                await audioFile.CopyToAsync(memoryStream);
                var audioBytes = memoryStream.ToArray();

                storyToEdit.AudioData = audioBytes;

                await _dbContext.SaveChangesAsync();

                return 1; 
            }

        }*/
    
    }
}
