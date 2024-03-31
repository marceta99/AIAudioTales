using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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

        public async Task<IEnumerable<User>> SearchWorkers(int countryId, int jobId)
        {
            var users = await _dbContext.Users.Where(u => u.CountryId == countryId && u.JobId == jobId)
                .ToListAsync();

            return users;

        }

        public async Task<Job> AddNewJob(JobCreateDTO job)
        {
            var newJob = new Job
            {
                JobName = job.JobName,
                CompanyId = job.CompanyId
            };
            var createdJob = _dbContext.Jobs.Add(newJob);
            await _dbContext.SaveChangesAsync();

            return createdJob.Entity;
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

        
    }
}
