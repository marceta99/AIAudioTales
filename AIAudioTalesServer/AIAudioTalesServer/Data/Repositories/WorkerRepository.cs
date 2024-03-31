using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AIAudioTalesServer.Data.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly AppDbContext _dbContext;

        public WorkerRepository(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IList<User>> GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users;
        }

        public async Task<Category> AddNewCategory(string categoryName, int jobId)
        {
            var category = new Category
            {
                CategoryName = categoryName,
                JobId = jobId
            };
            var createdCategory =  _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return createdCategory.Entity;
        }
        public async Task<CategoryItem> AddCategoryItem(CategoryItemCreateDTO item)
        {
            var newItem = new CategoryItem
            {
                Description = item.Description, 
                ItemName = item.ItemName, 
                CategoryId = item.CategoryId
            };
            var createdItem = _dbContext.CategoryItems.Add(newItem);
            await _dbContext.SaveChangesAsync();

            return createdItem.Entity;
        }

        public async Task<IList<Category>> GetCategoriesForJob(int jobId)
        {
            var categories = await _dbContext.Categories.Where(b => b.JobId == jobId)
                .ToListAsync();

            foreach (var category in categories)
            {
                var categoryItems = 
                    await _dbContext.CategoryItems.Where(i => i.CategoryId == category.Id).ToListAsync();
                
                category.CategoryItems = categoryItems;
            }

            return categories;
        }
    }
}
