using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.UpdateDepartment
{
    public record UpdateDepartmentCommand(Guid Id, string? Name, string? Description)
        : IRequest<Result<DepartmentResponse>>;
}
