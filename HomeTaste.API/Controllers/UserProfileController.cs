using HomeTaste.API.Wrappers;
using HomeTaste.Application.Features.UserProfile;
using HomeTaste.Application.Features.UserProfile.Commands.ChangePassword;
using HomeTaste.Application.Features.UserProfile.Commands.UpdateProfile;
using HomeTaste.Application.Features.UserProfile.Commands.UploadAvatar;
using HomeTaste.Application.Features.UserProfile.Queries.GetProfile;
using HomeTaste.Application.Interfaces.Order;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>Current user's own profile management.</summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderService _orderService;

        public UserProfileController(
            IMediator mediator,
            IOrderService orderService)
        {
            _mediator = mediator;
            _orderService = orderService;
        }

        /// <summary>Returns the current user's profile (name, email, phone, avatar, roles).</summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetProfileQuery());
            return Ok(result);
        }

        /// <summary>Updates editable profile fields (first/last name, date of birth, phone number).</summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await _mediator.Send(new UpdateProfileCommand(request));
            return Ok(result);
        }

        /// <summary>Changes the current user's password. Requires the existing password for verification.</summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(request));
            return Ok(result);
        }

        /// <summary>Uploads or replaces the current user's avatar image. Send as multipart/form-data with field name "file".</summary>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            await using var stream = file.OpenReadStream();
            var result = await _mediator.Send(new UploadAvatarCommand(stream, file.FileName, file.ContentType));
            return Ok(result);
        }

        /// <summary>Returns the current user's paginated order history.</summary>
        [HttpGet("order-history")]
        public async Task<IActionResult> GetOrderHistory(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // IOrderService is not yet converted to CQRS — still returns Result<T> with a
            // meaningful ResultType, so this action keeps using ApiResponseMapper, unlike
            // every other action in this controller.
            var result = await _orderService.GetMyOrdersAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
