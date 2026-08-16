using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICookieService _cookieService;

        public LogoutCommandHandler(IApplicationDbContext context, ICookieService cookieService)
        {
            _context = context;
            _cookieService = cookieService;
        }

        public async Task<Result<string>> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var refreshToken = await _cookieService.GetCookieAsync<string>("refresh_token");

            if (refreshToken == null)
                throw new UnauthorizedException("Refresh token not found");

            var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid refresh token.");

            tokenEntity.Revoke();

            await _context.SaveChangesAsync(cancellationToken);

            await _cookieService.RemoveCookieAsync("refresh_token");

            return Result<string>.Ok("Logout successful.", "Logout succeeded");
        }
    }
}
