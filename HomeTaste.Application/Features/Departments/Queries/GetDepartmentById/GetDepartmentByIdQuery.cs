using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Queries.GetDepartmentById
{
    public record GetDepartmentByIdQuery(Guid Id) : IRequest<Result<DepartmentResponse>>;
}
