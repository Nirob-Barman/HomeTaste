using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Queries.GetAllDepartments
{
    public record GetAllDepartmentsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<DepartmentResponse>>>>;
}
