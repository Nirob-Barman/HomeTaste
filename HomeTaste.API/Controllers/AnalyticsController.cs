using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Analytics.Queries.GetDailyRevenue;
using HomeTaste.Application.Features.Analytics.Queries.GetDashboard;
using HomeTaste.Application.Features.Analytics.Queries.GetTopCustomers;
using HomeTaste.Application.Features.Analytics.Queries.GetTopMeals;
using MediatR;
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
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Returns the full dashboard: KPI cards, status breakdown, top meals/customers, daily revenue chart, support/loyalty/inventory summaries.</summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _mediator.Send(new GetDashboardQuery());
            return Ok(result);
        }

        /// <summary>Returns daily revenue and order count for the last N days (default 30).</summary>
        [HttpGet("daily-revenue")]
        public async Task<IActionResult> GetDailyRevenue([FromQuery] int days = 30)
        {
            var result = await _mediator.Send(new GetDailyRevenueQuery { Days = days });
            return Ok(result);
        }

        /// <summary>Returns the top N meals by quantity ordered (default top 10).</summary>
        [HttpGet("top-meals")]
        public async Task<IActionResult> GetTopMeals([FromQuery] int top = 10)
        {
            var result = await _mediator.Send(new GetTopMealsQuery { Top = top });
            return Ok(result);
        }

        /// <summary>Returns the top N customers by total spend on delivered orders (default top 10).</summary>
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int top = 10)
        {
            var result = await _mediator.Send(new GetTopCustomersQuery { Top = top });
            return Ok(result);
        }
    }
}
