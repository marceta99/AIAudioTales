using AIAudioTalesServer.Models;

namespace AIAudioTalesServer.Data.Interfaces
{
    public interface IWorkerRepository
    {
        Task<IList<User>> GetAllUsers();
        Task<Category> AddNewCategory(string categoryName, int jobId);

        Task<IList<Category>> GetCategoriesForJob(int jobId);

        Task<CategoryItem> AddCategoryItem(CategoryItemCreateDTO item);
    }
}
