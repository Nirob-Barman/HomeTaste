using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Loyalty;
using HomeTaste.Application.Features.Loyalty.Commands.AdjustPoints;
using HomeTaste.Application.Features.Loyalty.Queries.GetAccountByUserId;
using HomeTaste.Application.Features.Loyalty.Queries.GetMyAccount;
using HomeTaste.Application.Features.Loyalty.Queries.GetMyTransactions;
using HomeTaste.Application.Features.Loyalty.Queries.PreviewRedemption;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoyaltyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Returns the loyalty account (points balance, tier, next-tier progress) for the current user.</summary>
        [HttpGet("my-account")]
        public async Task<IActionResult> GetMyAccount()
        {
            var result = await _mediator.Send(new GetMyAccountQuery());
            return Ok(result);
        }

        /// <summary>Returns paginated loyalty point transaction history for the current user.</summary>
        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetMyTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetMyTransactionsQuery { PageNumber = pageNumber, PageSize = pageSize });
            return Ok(result);
        }

        /// <summary>Previews how much discount redeeming a given number of points would yield.</summary>
        [HttpGet("preview-redemption")]
        public async Task<IActionResult> PreviewRedemption([FromQuery] int points)
        {
            var result = await _mediator.Send(new PreviewRedemptionQuery(points));
            return Ok(result);
        }

        /// <summary>Returns the loyalty account for a specific user. Admin only.</summary>
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet("account/{userId}")]
        public async Task<IActionResult> GetAccountByUserId(string userId)
        {
            var result = await _mediator.Send(new GetAccountByUserIdQuery(userId));
            return Ok(result);
        }

        /// <summary>Admin: manually add or deduct points from a user's loyalty account.</summary>
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustPoints([FromBody] AdjustPointsRequest request)
        {
            var result = await _mediator.Send(new AdjustPointsCommand(request));
            return Ok(result);
        }
    }
}
