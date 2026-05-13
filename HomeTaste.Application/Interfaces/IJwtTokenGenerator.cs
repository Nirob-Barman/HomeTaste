using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.DTOs.Common;
using HomeTaste.Domain.Entities;
using System.Security.Claims;

namespace HomeTaste.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<(string Token, DateTime ExpiresAt)> GenerateTokenAsync(ApplicationUser user);
        (string RefreshToken, DateTime ExpiresAt) GenerateRefreshTokenAsync(ApplicationUser user);
        ClaimsPrincipal VerifyAccessToken(string token);
        RefreshTokenData VerifyRefreshToken(string token);
        ClaimsPrincipal VerifyToken(string token, string secret);
    }
}
