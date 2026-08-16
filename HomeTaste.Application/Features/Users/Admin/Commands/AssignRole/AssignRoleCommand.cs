using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.AssignRole
{
    public record AssignRoleCommand(string? UserId, string? RoleName)
        : IRequest<Result<RoleAssignmentResponse>>;
}
