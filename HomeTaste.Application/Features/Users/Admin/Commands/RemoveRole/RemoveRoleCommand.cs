using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.RemoveRole
{
    public class RemoveRoleCommand : IRequest<Result<RoleRemovalResponse>>
    {
        public RemoveRoleRequest Request { get; set; }

        public RemoveRoleCommand(RemoveRoleRequest request)
        {
            Request = request;
        }
    }
}
