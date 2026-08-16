using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, Result<PaginatedResponse<IEnumerable<TaskResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllTasksQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<TaskResponse>>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(task =>
                    task.Title!.Contains(request.SearchTerm) ||
                    task.Description!.Contains(request.SearchTerm)
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            var tasks = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(task => new TaskResponse
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status
                })
                .ToListAsync(cancellationToken);

            paginationMeta.CurrentPageCount = tasks.Count;

            var response = new PaginatedResponse<IEnumerable<TaskResponse>>
            {
                MetaData = paginationMeta,
                Data = tasks,
            };

            return Result<PaginatedResponse<IEnumerable<TaskResponse>>>.Ok(response, "Tasks retrieved successfully");
        }
    }
}
