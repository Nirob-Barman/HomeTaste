using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = HomeTaste.Domain.Entities.RefreshToken;

namespace HomeTaste.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IApplicationDbContext _context;
        private readonly ICookieService _cookieService;

        public RefreshTokenCommandHandler(
            IUserManager userManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IApplicationDbContext context,
            ICookieService cookieService)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _context = context;
            _cookieService = cookieService;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            // Cookie is preferred (HttpOnly, secure); body token is a fallback for
            // development where cross-scheme cookie delivery can be unreliable.
            var refreshToken = await _cookieService.GetCookieAsync<string>("refresh_token")
                               ?? command.BodyRefreshToken;

            if (refreshToken == null)
                throw new UnauthorizedException("Refresh token not found");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

                if (tokenEntity == null)
                    throw new UnauthorizedException("Refresh token not found");

                if (tokenEntity.IsRevoked)
                    throw new UnauthorizedException("Refresh token not found");

                if (tokenEntity.ExpiryDate < DateTime.UtcNow)
                    throw new UnauthorizedException("Refresh token not found");

                tokenEntity.Revoke();

                var user = await _userManager.FindByIdAsync(tokenEntity.UserId!);

                var (newRefreshToken, refreshTokenExpiresAt) = _jwtTokenGenerator.GenerateRefreshTokenAsync(user!);
                var newRefreshEntity = RefreshTokenEntity.Create(newRefreshToken, tokenEntity.UserId, DateTime.UtcNow.AddDays(30));

                _context.RefreshTokens.Add(newRefreshEntity);

                if (user == null)
                    throw new NotFoundException("Refresh token not found");

                var (jwtToken, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user!);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                await _cookieService.SetCookieAsync("refresh_token", newRefreshToken, refreshTokenExpiresAt);

                var response = new AuthResponse
                {
                    AccessToken = jwtToken,
                    ExpiresAt = expiresAt,
                    RefreshToken = newRefreshToken,
                    Email = user.Email
                };

                return Result<AuthResponse>.Ok(response, "Token refreshed successfully");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
