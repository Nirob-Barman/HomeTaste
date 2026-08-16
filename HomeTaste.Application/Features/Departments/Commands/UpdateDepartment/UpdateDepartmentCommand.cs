using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest<Result<DepartmentResponse>>
    {
        public Guid Id { get; set; }
        public DepartmentRequest DepartmentRequest { get; set; }

        public UpdateDepartmentCommand(Guid id, DepartmentRequest departmentRequest)
        {
            Id = id;
            DepartmentRequest = departmentRequest;
        }
    }
}
