using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Users.Admin;
using HomeTaste.Application.Features.Users.Admin.Commands.AssignRole;
using HomeTaste.Application.Features.Users.Admin.Commands.BanUser;
using HomeTaste.Application.Features.Users.Admin.Commands.RemoveRole;
using HomeTaste.Application.Features.Users.Admin.Commands.UnbanUser;
using HomeTaste.Application.Features.Users.Admin.Queries.GetAllUsers;
using HomeTaste.Application.Features.Users.Admin.Queries.GetUserById;
using MediatR;
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
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Returns a paginated list of all users. Supports search by email, first name, or last name.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize   = 20,
            [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetAllUsersQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = search });
            return Ok(result);
        }

        /// <summary>Returns a single user by their identity ID.</summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            return Ok(result);
        }

        /// <summary>Bans a user (locks their account indefinitely).</summary>
        [HttpPost("{userId}/ban")]
        public async Task<IActionResult> BanUser(string userId, [FromBody] BanUserRequest request)
        {
            var result = await _mediator.Send(new BanUserCommand(userId, request));
            return Ok(result);
        }

        /// <summary>Lifts a ban from a user (unlocks their account).</summary>
        [HttpPost("{userId}/unban")]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var result = await _mediator.Send(new UnbanUserCommand(userId));
            return Ok(result);
        }

        /// <summary>Assigns a role to a user.</summary>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var result = await _mediator.Send(new AssignRoleCommand(request));
            return Ok(result);
        }

        /// <summary>Removes a role from a user.</summary>
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
        {
            var result = await _mediator.Send(new RemoveRoleCommand(request));
            return Ok(result);
        }
    }
}
