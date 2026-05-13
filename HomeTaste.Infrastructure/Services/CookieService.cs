using HomeTaste.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace HomeTaste.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private const string CookiePrefix = "HomeTaste_"; // Prefix for cookie keys to avoid name clashes
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetCookieAsync<T>(string key, T value, DateTime expiresAt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // Make cookie accessible only to HTTP requests (not JavaScript)
                Secure = true,    // Ensure cookie is sent over HTTPS only
                //SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt // Set expiration time for the cookie
            };

            // Serialize the object into JSON string
            var jsonValue = JsonSerializer.Serialize(value);

            // Set the cookie using the serialized JSON value
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(CookiePrefix + key, jsonValue, cookieOptions);
            await Task.CompletedTask;
        }

        public async Task<T?> GetCookieAsync<T>(string key)
        {
            // Retrieve the cookie value by key
            if (_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(CookiePrefix + key, out var cookieValue))
            {
                // Deserialize the cookie value to the specified type
                return JsonSerializer.Deserialize<T>(cookieValue);
            }
            await Task.CompletedTask;
            return default; // Return default if the cookie doesn't exist
        }

        public async Task RemoveCookieAsync(string key)
        {
            // Remove the cookie by key
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete(CookiePrefix + key);
            await Task.CompletedTask;
        }
    }
}
