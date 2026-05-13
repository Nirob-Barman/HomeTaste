using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return ApiResponseMapper.FromResult(this, result);

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        [Authorize]
        //[AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _authService.GetCurrentUserAsync();
            return ApiResponseMapper.FromResult(this, result);
        }


        [HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _authService.RefreshTokenAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var restult = await _authService.LogoutAsync();
            return ApiResponseMapper.FromResult(this, restult);
        }
    }
}
