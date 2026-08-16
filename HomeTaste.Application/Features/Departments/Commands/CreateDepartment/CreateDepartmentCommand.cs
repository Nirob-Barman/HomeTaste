using HomeTaste.Application.DTOs.OrganizationDepartment;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Result<DepartmentResponse>>
    {
        public DepartmentRequest DepartmentRequest { get; set; }

        public CreateDepartmentCommand(DepartmentRequest departmentRequest)
        {
            DepartmentRequest = departmentRequest;
        }
    }
}
