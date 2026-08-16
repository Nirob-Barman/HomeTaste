using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities;
using MediatR;
using RefreshTokenEntity = HomeTaste.Domain.Entities.RefreshToken;

namespace HomeTaste.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly ISignInManager _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ICookieService _cookieService;

        public LoginCommandHandler(
            IUserManager userManager,
            ISignInManager signInManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IApplicationDbContext context,
            IUserContextService userContextService,
            ICookieService cookieService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _context = context;
            _userContextService = userContextService;
            _cookieService = cookieService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var user = await _userManager.FindByEmailAsync(request.Email!);

            if (user == null)
            {
                var failedAudit = LoginAudit.Create(null, false, _userContextService.IpAddress, _userContextService.UserAgent);
                _context.LoginAudits.Add(failedAudit);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                throw new UnauthorizedException("Invalid username");
            }

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, request.Password!);

            var loginAudit = LoginAudit.Create(user.Id, passwordValid, _userContextService.IpAddress, _userContextService.UserAgent);
            _context.LoginAudits.Add(loginAudit);

            if (!passwordValid)
            {
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                throw new UnauthorizedException("Invalid password");
            }

            var (refreshToken, refreshTokenExpiresAt) = _jwtTokenGenerator.GenerateRefreshTokenAsync(user!);
            var refreshEntity = RefreshTokenEntity.Create(refreshToken, user.Id, refreshTokenExpiresAt);
            _context.RefreshTokens.Add(refreshEntity);

            var (jwtToken, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user!);

            await _cookieService.SetCookieAsync("refresh_token", refreshToken, refreshTokenExpiresAt);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new AuthResponse
            {
                AccessToken = jwtToken,
                ExpiresAt = expiresAt,
                RefreshToken = refreshToken,
                Email = user!.Email!,
            };

            return Result<AuthResponse>.Ok(response, "Login successful");
        }
    }
}
