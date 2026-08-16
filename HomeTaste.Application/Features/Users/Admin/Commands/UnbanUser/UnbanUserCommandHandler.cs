using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.UnbanUser
{
    public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public UnbanUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(UnbanUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var (succeeded, errors) = await _userManager.SetLockoutAsync(command.UserId, false);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<bool>.Ok(true, "User unbanned successfully");
        }
    }
}
