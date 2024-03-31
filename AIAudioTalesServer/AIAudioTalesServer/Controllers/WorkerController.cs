using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Data.Repositories;
using AIAudioTalesServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerRepository workerRepository;

        public WorkerController(IWorkerRepository workerRepository)
        {
            this.workerRepository = workerRepository;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IList<User>>> GetAllUsers()
        {
            var users = await workerRepository.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("AddNewCategory")]
        public async Task<ActionResult<Category>> AddNewCategory([FromBody] CategoryCreateDTO category)
        {

            if (ModelState.IsValid)
            {
                var newCategory = await workerRepository.AddNewCategory(category.CategoryName, category.JobId);
                return Ok(newCategory);
            }
            return BadRequest();
        }

        [HttpGet("GetCategoriesForJob")]
        public async Task<ActionResult<IList<Category>>> GetCategoriesForJob(int jobId)
        {
            var categories = await workerRepository.GetCategoriesForJob(jobId);
        
            return Ok(categories);
        }

        [HttpPost("AddCategoryItem")]
        public async Task<ActionResult<Category>> AddCategoryItem([FromBody] CategoryItemCreateDTO item)
        {

            if (ModelState.IsValid)
            {
                var newItem = await workerRepository.AddCategoryItem(item);
                return Ok(newItem);
            }
            return BadRequest();
        }
    }
}
