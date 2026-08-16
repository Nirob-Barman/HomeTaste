using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(string? BodyRefreshToken = null)
        : IRequest<Result<AuthResponse>>;
}
