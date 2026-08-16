using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using TaskEntity = HomeTaste.Domain.Entities.Tasks.Tasks;

namespace HomeTaste.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TaskResponse>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var task = TaskEntity.Create(request.Title, request.Description, request.DueDate, request.Priority, request.Status);

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            var taskResponse = new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status
            };

            return Result<TaskResponse>.Ok(taskResponse, "Task created successfully");
        }
    }
}
