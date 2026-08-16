using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.BanUser
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;

        public BanUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(BanUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var (succeeded, errors) = await _userManager.SetLockoutAsync(command.UserId, true);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<bool>.Ok(true, "User banned successfully");
        }
    }
}
