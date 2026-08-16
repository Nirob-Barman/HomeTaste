using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.AssignRole
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result<RoleAssignmentResponse>>
    {
        private readonly IUserManager _userManager;

        public AssignRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<RoleAssignmentResponse>> Handle(AssignRoleCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId!);
            if (user == null)
                throw new NotFoundException("User not found");

            var (succeeded, errors) = await _userManager.AddToRoleAsync(user, command.RoleName!);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<RoleAssignmentResponse>.Ok(
                new RoleAssignmentResponse { UserId = command.UserId, RoleName = command.RoleName },
                "Role assigned successfully");
        }
    }
}
