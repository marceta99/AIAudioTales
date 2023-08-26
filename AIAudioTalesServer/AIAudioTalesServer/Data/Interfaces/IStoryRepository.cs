using AIAudioTalesServer.Models;
using AIAudioTalesServer.Models.DTOS.Incoming;
using AIAudioTalesServer.Models.DTOS.Outgoing;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IStoryRepository
    {
        Task<byte[]?> GetStoryAudio(int storyId);
        Task<IList<StoryReturnDTO>> GetAllStoriesForBook(int bookId);
        Task<StoryReturnDTO> GetStory(int storyId);
        Task<StoryReturnDTO> AddNewStoryToBook(int bookId,StoryCreationDTO newStory);

        Task<int> UpdateStoryDetails(StoryUpdateDTO story);
        Task<int> DeleteStory(int storyId);
        Task<int> UploadAudioForStory(int storyId, IFormFile imageFile);
        
    }
}
