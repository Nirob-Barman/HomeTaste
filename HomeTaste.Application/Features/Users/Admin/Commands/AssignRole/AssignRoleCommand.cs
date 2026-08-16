using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.AssignRole
{
    public class AssignRoleCommand : IRequest<Result<RoleAssignmentResponse>>
    {
        public AssignRoleRequest Request { get; set; }

        public AssignRoleCommand(AssignRoleRequest request)
        {
            Request = request;
        }
    }
}
