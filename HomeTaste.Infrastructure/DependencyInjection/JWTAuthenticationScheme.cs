using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HomeTaste.Infrastructure.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            // add JWT service

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(config["JwtSettings:AccessToken:Key"]!);
                var issuer = config["JwtSettings:AccessToken:Issuer"];
                var audience = config["JwtSettings:AccessToken:Audience"];
                //var expiryMinutes = int.Parse(config["JwtSettings:AccessToken:ExpiryMinutes"]!);
                //var signingAlgorithm = config["JwtSettings:AccessToken:SigningAlgorithm"];

                options.RequireHttpsMetadata = true;
                options.SaveToken = true; // Save the JWT token in AuthenticationProperties

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Ensure token expiration is validated
                    ValidateIssuerSigningKey = true, // Validate the signing key
                    ClockSkew = TimeSpan.Zero, // No tolerance for expiration time,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    // Allow SignalR WebSocket connections to pass the JWT via query string
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    },

                    // 1. Handles missing or invalid tokens (triggered before controller)
                    OnChallenge = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var errorDescription = context.ErrorDescription ?? "Authentication failed or token is missing.";

                            var result = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                statusCode = 401,
                                message = "Unauthorized. Please provide a valid token.",
                                success = false,
                                data = (object)null!,
                                errors = new[] { errorDescription }
                            });

                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },

                    // 2. Handles other JWT failures (e.g., token is malformed or invalid)
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            statusCode = 401,
                            message = "Token validation failed.",
                            success = false,
                            data = (object)null!,
                            errors = new[] { context.Exception.Message }
                        });

                        return context.Response.WriteAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            statusCode = 403,
                            message = "Forbidden. You do not have permission to access this resource.",
                            success = false,
                            data = (object)null!,
                            errors = new[] { "Insufficient role or permission." }
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            });

            return services;
        }
    }
}
