using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Tasks.Queries.GetAllTasks
{
    public record GetAllTasksQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<TaskResponse>>>>;
}
