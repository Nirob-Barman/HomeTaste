using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
    {
        public string? BodyRefreshToken { get; set; }

        public RefreshTokenCommand(string? bodyRefreshToken = null)
        {
            BodyRefreshToken = bodyRefreshToken;
        }
    }
}
