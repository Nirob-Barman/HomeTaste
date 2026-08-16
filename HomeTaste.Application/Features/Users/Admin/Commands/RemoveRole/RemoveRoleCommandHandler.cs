using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.RemoveRole
{
    public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result<RoleRemovalResponse>>
    {
        private readonly IUserManager _userManager;

        public RemoveRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<RoleRemovalResponse>> Handle(RemoveRoleCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var user = await _userManager.FindByIdAsync(request.UserId!);
            if (user == null)
                throw new NotFoundException("User not found");

            var (succeeded, errors) = await _userManager.RemoveFromRoleAsync(user, request.RoleName!);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<RoleRemovalResponse>.Ok(
                new RoleRemovalResponse { UserId = request.UserId, RoleName = request.RoleName },
                "Role removed successfully");
        }
    }
}
