using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>Admin analytics dashboard endpoints.</summary>
    [Authorize(Policy = Policies.AdminOnly)]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>Returns the full dashboard: KPI cards, status breakdown, top meals/customers, daily revenue chart, support/loyalty/inventory summaries.</summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _analyticsService.GetDashboardStatsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns daily revenue and order count for the last N days (default 30).</summary>
        [HttpGet("daily-revenue")]
        public async Task<IActionResult> GetDailyRevenue([FromQuery] int days = 30)
        {
            var result = await _analyticsService.GetDailyRevenueAsync(days);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns the top N meals by quantity ordered (default top 10).</summary>
        [HttpGet("top-meals")]
        public async Task<IActionResult> GetTopMeals([FromQuery] int top = 10)
        {
            var result = await _analyticsService.GetTopMealsAsync(top);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns the top N customers by total spend on delivered orders (default top 10).</summary>
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int top = 10)
        {
            var result = await _analyticsService.GetTopCustomersAsync(top);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
