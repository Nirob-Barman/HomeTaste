using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public ChangePasswordCommandHandler(IUserManager userManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Unauthorized");

            var (succeeded, errors) = await _userManager.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<bool>.Ok(true, "Password changed successfully");
        }
    }
}
