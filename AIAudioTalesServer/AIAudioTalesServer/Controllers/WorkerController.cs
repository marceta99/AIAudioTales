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

        [HttpGet("GetCategoriesForJob")]
        public async Task<ActionResult<IList<Category>>> GetCategoriesForJob(int jobId)
        {
            var categories = await workerRepository.GetCategoriesForJob(jobId);

            return Ok(categories);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IList<User>>> SearchWorkers([FromQuery] int countryId = 1, [FromQuery] int jobId = 1)
        {
            var workers = await workerRepository.SearchWorkers(countryId, jobId);
            return Ok(workers);
        }

        [HttpPost("AddNewJob")]
        public async Task<ActionResult<Job>> AddNewJob([FromBody] JobCreateDTO job)
        {

            if (ModelState.IsValid)
            {
                var newJob = await workerRepository.AddNewJob(job);
                return Ok(newJob);
            }
            return BadRequest();
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
