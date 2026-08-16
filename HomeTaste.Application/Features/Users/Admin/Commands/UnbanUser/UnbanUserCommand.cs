using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.UnbanUser
{
    public record UnbanUserCommand(string UserId)
        : IRequest<Result<bool>>;
}
