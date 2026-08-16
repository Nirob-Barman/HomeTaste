using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.BanUser
{
    public record BanUserCommand(string UserId, string? Reason)
        : IRequest<Result<bool>>;
}
