using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password)
        : IRequest<Result<AuthResponse>>;
}
