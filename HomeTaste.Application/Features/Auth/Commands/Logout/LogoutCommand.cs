using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand : IRequest<Result<string>>;
}
