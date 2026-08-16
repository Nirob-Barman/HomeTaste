using HomeTaste.Application.Features.Auth;
using HomeTaste.Application.Features.Auth.Commands.Login;
using HomeTaste.Application.Features.Auth.Commands.Logout;
using HomeTaste.Application.Features.Auth.Commands.RefreshToken;
using HomeTaste.Application.Features.Auth.Commands.Register;
using HomeTaste.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _mediator.Send(new RegisterCommand(request));
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _mediator.Send(new LoginCommand(request));
            return Ok(result);
        }

        [Authorize]
        //[AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());
            return Ok(result);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest? request = null)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(request?.RefreshToken));
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            return Ok(result);
        }
    }
}
