using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.DeleteDepartment
{
    public record DeleteDepartmentCommand(Guid Id) : IRequest<Result<bool>>;
}
