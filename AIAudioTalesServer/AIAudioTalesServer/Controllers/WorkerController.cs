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
    }
}
