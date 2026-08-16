using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.ChangePassword
{
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword)
        : IRequest<Result<bool>>;
}
