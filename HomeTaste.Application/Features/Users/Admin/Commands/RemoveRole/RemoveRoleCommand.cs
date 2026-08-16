using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.RemoveRole
{
    public record RemoveRoleCommand(string? UserId, string? RoleName)
        : IRequest<Result<RoleRemovalResponse>>;
}
