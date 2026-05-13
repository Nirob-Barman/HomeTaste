using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Loyalty;
using HomeTaste.Application.Interfaces.Loyalty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService;

        public LoyaltyController(ILoyaltyService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        /// <summary>Returns the loyalty account (points balance, tier, next-tier progress) for the current user.</summary>
        [HttpGet("my-account")]
        public async Task<IActionResult> GetMyAccount()
        {
            var result = await _loyaltyService.GetMyAccountAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns paginated loyalty point transaction history for the current user.</summary>
        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetMyTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _loyaltyService.GetMyTransactionsAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Previews how much discount redeeming a given number of points would yield.</summary>
        [HttpGet("preview-redemption")]
        public async Task<IActionResult> PreviewRedemption([FromQuery] int points)
        {
            var result = await _loyaltyService.PreviewRedemptionAsync(points);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns the loyalty account for a specific user. Admin only.</summary>
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet("account/{userId}")]
        public async Task<IActionResult> GetAccountByUserId(string userId)
        {
            var result = await _loyaltyService.GetAccountByUserIdAsync(userId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Admin: manually add or deduct points from a user's loyalty account.</summary>
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustPoints([FromBody] AdjustPointsRequest request)
        {
            var result = await _loyaltyService.AdjustPointsAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
