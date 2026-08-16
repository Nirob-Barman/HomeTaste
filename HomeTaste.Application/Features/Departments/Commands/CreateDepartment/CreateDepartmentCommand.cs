using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.CreateDepartment
{
    public record CreateDepartmentCommand(string? Name, string? Description)
        : IRequest<Result<DepartmentResponse>>;
}
