using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Admin;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>Admin: user management — list, ban/unban, role assignment.</summary>
    [Authorize(Policy = Policies.AdminOnly)]
    [Route("api/admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        /// <summary>Returns a paginated list of all users. Supports search by email, first name, or last name.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize   = 20,
            [FromQuery] string? search = null)
        {
            var result = await _adminUserService.GetAllUsersAsync(pageNumber, pageSize, search);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns a single user by their identity ID.</summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _adminUserService.GetUserByIdAsync(userId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Bans a user (locks their account indefinitely).</summary>
        [HttpPost("{userId}/ban")]
        public async Task<IActionResult> BanUser(string userId, [FromBody] BanUserRequest request)
        {
            var result = await _adminUserService.BanUserAsync(userId, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Lifts a ban from a user (unlocks their account).</summary>
        [HttpPost("{userId}/unban")]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var result = await _adminUserService.UnbanUserAsync(userId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Assigns a role to a user.</summary>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var result = await _adminUserService.AssignRoleAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Removes a role from a user.</summary>
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
        {
            var result = await _adminUserService.RemoveRoleAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
