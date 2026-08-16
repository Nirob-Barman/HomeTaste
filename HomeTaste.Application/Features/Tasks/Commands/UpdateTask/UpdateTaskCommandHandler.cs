using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<TaskResponse>> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var task = await _context.Tasks.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (task == null)
                throw new NotFoundException("Task not found");

            task.UpdateDetails(request.Title, request.Description, request.DueDate, request.Priority, request.Status);

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

            return Result<TaskResponse>.Ok(taskResponse, "Task updated successfully");
        }
    }
}
