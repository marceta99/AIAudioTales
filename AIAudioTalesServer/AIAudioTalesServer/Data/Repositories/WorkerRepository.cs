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
    }
}
