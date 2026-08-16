using HomeTaste.Application.Common.Exceptions;
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
            var departmentResponses = await _context.Departments
                .Select(department => new DepartmentResponse
                {
                    Id = department.Id,
                    Name = department.Name,
                    Description = department.Description
                })
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                departmentResponses = departmentResponses.Where(department =>
                    department.Name!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    department.Description!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = departmentResponses.Count();

            var pagedDepartments = departmentResponses
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            var currentPageCount = pagedDepartments.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<DepartmentResponse>>
            {
                Data = pagedDepartments,
                MetaData = paginationMeta
            };

            if (!pagedDepartments.Any())
            {
                throw new NotFoundException("No departments found");
            }

            return Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>.Ok(response, "Departments retrieved successfully");
        }
    }
}
