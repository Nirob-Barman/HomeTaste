using HomeTaste.Application.DTOs.Common;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthGuard.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IUserManager _userManager;

        public JwtTokenGenerator(IConfiguration configuration, IUserManager userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<(string Token, DateTime ExpiresAt)> GenerateTokenAsync(ApplicationUser user)
        {
            // Accessing nested "AccessToken" settings
            var accessTokenSettings = _configuration.GetSection("JwtSettings:AccessToken");

            // Access individual properties
            var secret = accessTokenSettings["Key"];
            var issuer = accessTokenSettings["Issuer"];
            var audience = accessTokenSettings["Audience"];
            var expiryMinutes = int.Parse(accessTokenSettings["ExpiryMinutes"]!);  // Assuming this exists in the config


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email!)
            }.ToList();

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiresAt,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        public (string RefreshToken, DateTime ExpiresAt) GenerateRefreshTokenAsync(ApplicationUser user)
        {
            // Accessing nested "RefreshToken" settings
            var refreshTokenSettings = _configuration.GetSection("JwtSettings:RefreshToken");

            // Access individual properties
            var secret = refreshTokenSettings["Key"];
            var issuer = refreshTokenSettings["Issuer"];
            var audience = refreshTokenSettings["Audience"];
            var expiryMinutes = int.Parse(refreshTokenSettings["ExpiryMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Refresh token will generally contain the user ID and issue time
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            }.ToList();

            // Calculate the expiration time for the refresh token
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiresAt,
                signingCredentials: creds);

            // Return the refresh token and its expiration time
            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }



        // Method to verify the Access Token
        public ClaimsPrincipal VerifyAccessToken(string token)
        {
            var accessTokenSettings = _configuration.GetSection("JwtSettings:AccessToken");

            var secret = accessTokenSettings["Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException("Access token validation failed.", ex);
            }
        }

        // Method to verify the Refresh Token
        public RefreshTokenData VerifyRefreshToken(string token)
        {
            var refreshTokenSettings = _configuration.GetSection("JwtSettings:RefreshToken");

            var secret = refreshTokenSettings["Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out _);
                //return principal;

                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var issueTimeClaim = principal.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;

                DateTime issueTime = DateTime.MinValue;
                if (DateTime.TryParse(issueTimeClaim, out issueTime))
                {
                    // Successfully parsed the issue time
                }

                // Return the extracted claims as a structured object
                return new RefreshTokenData
                {
                    UserId = userId,
                    IssueTime = issueTime
                };
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException("Refresh token validation failed.", ex);
            }
        }

        // Method to verify the JWT Token
        public ClaimsPrincipal VerifyToken(string token, string secret)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                throw new SecurityTokenException("Invalid token.");

            // Token Validation Parameters
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateIssuer = false,  // Set to true if you want to validate the issuer
                ValidateAudience = false,  // Set to true if you want to validate the audience
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                // Validate the token and get the claims principal
                var principal = handler.ValidateToken(token, validationParameters, out _);

                // Returning the principal with the claims
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException("Token validation failed.", ex);
            }
        }
    }
}
