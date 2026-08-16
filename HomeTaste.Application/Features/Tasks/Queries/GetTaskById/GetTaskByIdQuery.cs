using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskResponse>>;
}
