using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.TaskManagement;
using HomeTaste.Application.Interfaces.TaskManagement;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // Get all tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] int pageNumber = 1,
            int pageSize = 10, 
            string searchTerm = null!)
        {
            var result = await _taskService.GetAllTasksAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get task by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskRequest taskRequest)
        {
            var result = await _taskService.CreateTaskAsync(taskRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskRequest taskRequest)
        {
            var result = await _taskService.UpdateTaskAsync(id, taskRequest);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete a task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedTasks()
        {
            var result = await _taskService.BulkInsertPredefinedTasksAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
