using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Application.Interfaces.UserProfile;
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
        private readonly IUserProfileService _userProfileService;
        private readonly IOrderService       _orderService;

        public UserProfileController(
            IUserProfileService userProfileService,
            IOrderService orderService)
        {
            _userProfileService = userProfileService;
            _orderService       = orderService;
        }

        /// <summary>Returns the current user's profile (name, email, phone, avatar, roles).</summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _userProfileService.GetProfileAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates editable profile fields (first/last name, date of birth, phone number).</summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await _userProfileService.UpdateProfileAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Changes the current user's password. Requires the existing password for verification.</summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userProfileService.ChangePasswordAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Uploads or replaces the current user's avatar image. Send as multipart/form-data with field name "file".</summary>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            await using var stream = file.OpenReadStream();
            var result = await _userProfileService.UploadAvatarAsync(stream, file.FileName, file.ContentType);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns the current user's paginated order history.</summary>
        [HttpGet("order-history")]
        public async Task<IActionResult> GetOrderHistory(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetMyOrdersAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
