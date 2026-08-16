using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteDepartmentCommand(Guid id)
        {
            Id = id;
        }
    }
}
