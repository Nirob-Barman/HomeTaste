using HomeTaste.Application.Common;
using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Validators.Auth;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities;

namespace HomeTaste.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly ISignInManager _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly IRepository<LoginAudit> _loginAuditRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly ICookieService _cookieService;

        public AuthService(
            IUserManager userManager,
            IRoleManager roleManager,
            ISignInManager signInManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService, 
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            IRepository<LoginAudit> loginAuditRepository,
            IRepository<RefreshToken> refreshTokenRepository,
            ICookieService cookieService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _loginAuditRepository = loginAuditRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _cookieService = cookieService;
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var validationErrors = RegisterRequestValidator.Validate(request);
            if (validationErrors.Any())
            {
                return Result<RegisterResponse>.Fail(validationErrors, MessageConstants.Login.ValidationFailed, ResultType.ValidationError);
            }

            //if (!_userContextService.IsAuthenticated)
            //{
            //    return Result<RegisterResponse>.Fail("You must be logged in as an Admin to register new users.", "You must be logged in as an Admin to register new users.", ResultType.Unauthorized);
            //}

            //if (!_userContextService.IsInRole("Admin"))
            //{
            //    return Result<RegisterResponse>.Fail("Only Admins can register new users.");
            //}

            if (request.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return Result<RegisterResponse>.Fail("Registration failed", "You are not allowed to assign the 'Admin' role.", ResultType.Unauthorized);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            if (existingUser != null)
                return Result<RegisterResponse>.Fail("A user with this email already exists.", "Registration failed", ResultType.Conflict);

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
            };

            await _unitOfWork.BeginTransaction();

            var (success, userId, errors) = await _userManager.CreateAsync(user, request.Password!);
            if (!success)
            {
                await _unitOfWork.RollbackAsync();
                return Result<RegisterResponse>.Fail($"User creation failed: {string.Join(", ", errors)}", "Registration failed", ResultType.ValidationError);
            }


            //var createdUser = await _userManager.FindByIdAsync(userId!);
            //if (createdUser == null)
            //{
            //    await _unitOfWork.RollbackAsync();
            //    return Result<RegisterResponse>.Fail("User creation failed: unable to fetch user after creation.", "Registration failed", ResultType.Failure);
            //}                


            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var (roleAssignSuccess, roleErrors) = await _userManager.AddToRoleAsync(user, request.Role!);
                //var (roleAssignSuccess, roleErrors) = await _userManager.AddToRoleAsync(createdUser, request.Role!);
                if (!roleAssignSuccess)
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<RegisterResponse>.Fail($"Role assignment failed: {string.Join(", ", roleErrors)}", "Role assignment failed", ResultType.Failure);
                }
            }

            await _unitOfWork.CommitAsync();

            var registerResponse = new RegisterResponse
            {
                Id = userId!,
                Email = request.Email!,
                Role = request.Role
            };

            return Result<RegisterResponse>.Ok(registerResponse, "User registered successfully", ResultType.Success);
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var validationErrors = LoginRequestValidator.Validate(request);
            if (validationErrors.Any())
            {
                return Result<AuthResponse>.Fail(validationErrors, "Validation failed", ResultType.ValidationError);
            }

            await _unitOfWork.BeginTransaction();

            var user = await _userManager.FindByEmailAsync(request.Email!);

            var loginAudit = new LoginAudit
            {
                UserId = user?.Id,
                LoginTime = DateTime.UtcNow,
                IsSuccessful = false, // Will be updated after password check
                IPAddress = _userContextService.IpAddress,
                UserAgent = _userContextService.UserAgent,
            };

            if (user == null)
            {
                await _loginAuditRepository.AddAsync(loginAudit);
                await _unitOfWork.CommitAsync();
                return Result<AuthResponse>.Fail("Invalid username", "Invalid username", ResultType.Unauthorized);
            }

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, request.Password!);

            loginAudit.IsSuccessful = passwordValid;
            await _loginAuditRepository.AddAsync(loginAudit);

            if (!passwordValid)
            {
                await _unitOfWork.CommitAsync();
                return Result<AuthResponse>.Fail("Invalid password", "Invalid password", ResultType.Unauthorized);
            }

            var (refreshToken, refreshTokenExpiresAt) = _jwtTokenGenerator.GenerateRefreshTokenAsync(user!);
            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user?.Id,
                ExpiryDate = refreshTokenExpiresAt,
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshEntity);

            var (jwtToken, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user!);

            await _cookieService.SetCookieAsync("refresh_token", refreshToken, refreshTokenExpiresAt);


            await _unitOfWork.CommitAsync();

            var response = new AuthResponse
            {
                AccessToken = jwtToken,
                ExpiresAt = expiresAt,
                RefreshToken = refreshToken,
                Email = user!.Email!,
            };

            return Result<AuthResponse>.Ok(response, "Login successful", ResultType.Success);
        }

        public async Task<Result<UserProfileResponse>> GetCurrentUserAsync()
        {
            var userId = _userContextService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Result<UserProfileResponse>.Fail("User not found", "User ID not found.", ResultType.NotFound);
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Result<UserProfileResponse>.Fail("User not found.", "User not found", ResultType.NotFound);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userProfileResponse = new UserProfileResponse
            {
                Id              = user?.Id!,
                Email           = user?.Email!,
                FirstName       = user?.FirstName,
                LastName        = user?.LastName,
                DateOfBirth     = user?.DateOfBirth,
                PhoneNumber     = user?.PhoneNumber,
                ProfileImageUrl = user?.ProfileImageUrl,
                Roles           = roles.ToList()
            };

            return Result<UserProfileResponse>.Ok(userProfileResponse, "User profile retrieved successfully", ResultType.Success);
        }


        public async Task<Result<AuthResponse>> RefreshTokenAsync(string? bodyRefreshToken = null)
        {
            // Cookie is preferred (HttpOnly, secure); body token is a fallback for
            // development where cross-scheme cookie delivery can be unreliable.
            var refreshToken = await _cookieService.GetCookieAsync<string>("refresh_token")
                               ?? bodyRefreshToken;

            if (refreshToken == null)
                return Result<AuthResponse>.Fail("Refresh token not found", "Refresh token not found", ResultType.Unauthorized);

            await _unitOfWork.BeginTransaction();

            try
            {
                var tokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(r => r.Token == refreshToken);

                if (tokenEntity == null)
                    return Result<AuthResponse>.Fail("Refresh token not found", "Refresh token not found", ResultType.Unauthorized);

                if (tokenEntity.IsRevoked)
                    return Result<AuthResponse>.Fail("Refresh token revoked", "Refresh token not found", ResultType.Unauthorized);

                if (tokenEntity.ExpiryDate < DateTime.UtcNow)
                    return Result<AuthResponse>.Fail("Refresh token expired", "Refresh token not found", ResultType.Unauthorized);

                tokenEntity.IsRevoked = true;
                _refreshTokenRepository.Update(tokenEntity);

                
                //var refreshTokenData = _jwtTokenGenerator.VerifyRefreshToken(refreshToken!);

                var user = await _userManager.FindByIdAsync(tokenEntity.UserId!);
                //var user = await _userManager.FindByIdAsync(refreshTokenData.UserId!);

                var (newRefreshToken, refreshTokenExpiresAt) = _jwtTokenGenerator.GenerateRefreshTokenAsync(user!);
                var newRefreshEntity = new RefreshToken
                {
                    UserId = tokenEntity.UserId,
                    Token = newRefreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    IsRevoked = false
                };

                await _refreshTokenRepository.AddAsync(newRefreshEntity);                

                //var user = await _userManager.FindByIdAsync(tokenEntity.UserId!);

                if (user == null) return Result<AuthResponse>.Fail("User not found", "Refresh token not found", ResultType.NotFound);
                
                var (jwtToken, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user!);

                await _unitOfWork.CommitAsync();

                await _cookieService.SetCookieAsync("refresh_token", newRefreshToken, refreshTokenExpiresAt);

                var response = new AuthResponse
                {
                    AccessToken = jwtToken,
                    ExpiresAt = expiresAt,
                    RefreshToken = newRefreshToken,
                    Email = user.Email
                };

                return Result<AuthResponse>.Ok(response, "Token refreshed successfully", ResultType.Success);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }


        public async Task<Result<string>> LogoutAsync()
        {
            //if (string.IsNullOrWhiteSpace(request.RefreshToken))
            //{
            //    return Result<string>.Fail("Refresh token must not be empty.", "Validation failed", ResultType.ValidationError);
            //}

            var refreshToken = await _cookieService.GetCookieAsync<string>("refresh_token");

            if (refreshToken == null)
                return Result<string>.Fail("Refresh token not found", "Refresh token not found", ResultType.Unauthorized);

            var tokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
                return Result<string>.Fail("Invalid refresh token.", "Invalid refresh token", ResultType.Unauthorized);

            tokenEntity.IsRevoked = true;
            _refreshTokenRepository.Update(tokenEntity);

            await _unitOfWork.SaveChangesAsync();

            await _cookieService.RemoveCookieAsync("refresh_token");

            return Result<string>.Ok("Logout successful.", "Logout succeeded", ResultType.Success);
        }
    }
}
