using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDepartmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(department =>
                    department.Name!.Contains(request.SearchTerm) ||
                    department.Description!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var departmentResponses = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(department => new DepartmentResponse
                {
                    Id = department.Id,
                    Name = department.Name,
                    Description = department.Description
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = departmentResponses.Count;

            var response = new PaginatedResponse<IEnumerable<DepartmentResponse>>
            {
                Data = departmentResponses,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>.Ok(response, "Departments retrieved successfully");
        }
    }
}
