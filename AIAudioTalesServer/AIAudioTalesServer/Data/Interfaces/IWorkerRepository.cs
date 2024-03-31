using AIAudioTalesServer.Models;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IWorkerRepository
    {
        Task<IList<User>> GetAllUsers();
        Task<Category> AddNewCategory(string categoryName, int jobId);

        Task<IList<Category>> GetCategoriesForJob(int jobId);

        Task<IEnumerable<User>> SearchWorkers(int countryId, int jobId);

        Task<CategoryItem> AddCategoryItem(CategoryItemCreateDTO item);

        Task<Job> AddNewJob(JobCreateDTO job);
    }
}
