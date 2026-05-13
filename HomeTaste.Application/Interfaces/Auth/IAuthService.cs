using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<UserProfileResponse>> GetCurrentUserAsync();
        //Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<Result<AuthResponse>> RefreshTokenAsync();
        Task<Result<string>> LogoutAsync();
        
    }
}
