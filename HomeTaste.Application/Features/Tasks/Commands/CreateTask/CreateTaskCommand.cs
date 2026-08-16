using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(string? Title, string? Description, DateTime DueDate, TaskPriority Priority, TasksStatus Status)
        : IRequest<Result<TaskResponse>>;
}
