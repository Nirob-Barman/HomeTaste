using HomeTaste.Application.Features.Tasks;
using HomeTaste.Application.Features.Tasks.Commands.BulkInsertTasks;
using HomeTaste.Application.Features.Tasks.Commands.CreateTask;
using HomeTaste.Application.Features.Tasks.Commands.DeleteTask;
using HomeTaste.Application.Features.Tasks.Commands.UpdateTask;
using HomeTaste.Application.Features.Tasks.Queries.GetAllTasks;
using HomeTaste.Application.Features.Tasks.Queries.GetTaskById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get all tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllTasksQuery(pageNumber, pageSize, searchTerm));
            return Ok(result);
        }

        // Get task by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery(id));
            return Ok(result);
        }

        // Create a new task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskRequest taskRequest)
        {
            var result = await _mediator.Send(new CreateTaskCommand(taskRequest.Title, taskRequest.Description, taskRequest.DueDate, taskRequest.Priority, taskRequest.Status));
            return Ok(result);
        }

        // Update an existing task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskRequest taskRequest)
        {
            var result = await _mediator.Send(new UpdateTaskCommand(id, taskRequest.Title, taskRequest.Description, taskRequest.DueDate, taskRequest.Priority, taskRequest.Status));
            return Ok(result);
        }

        // Delete a task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _mediator.Send(new DeleteTaskCommand(id));
            return Ok(result);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertPredefinedTasks()
        {
            var result = await _mediator.Send(new BulkInsertTasksCommand());
            return Ok(result);
        }
    }
}
