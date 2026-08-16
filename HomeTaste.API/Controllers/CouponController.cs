using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Coupons;
using HomeTaste.Application.Features.Coupons.Commands.CreateCoupon;
using HomeTaste.Application.Features.Coupons.Commands.DeleteCoupon;
using HomeTaste.Application.Features.Coupons.Commands.ToggleCouponActive;
using HomeTaste.Application.Features.Coupons.Commands.UpdateCoupon;
using HomeTaste.Application.Features.Coupons.Queries.GetAllCoupons;
using HomeTaste.Application.Features.Coupons.Queries.GetCouponById;
using HomeTaste.Application.Features.Coupons.Queries.ValidateCoupon;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Manages discount coupons. Admin CRUD, public validation endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CouponController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets all coupons with optional search and pagination. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllCouponsQuery(pageNumber, pageSize, searchTerm));
            return Ok(result);
        }

        /// <summary>Gets a coupon by ID. Admin only.</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCouponByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Creates a new coupon. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CouponRequest request)
        {
            var result = await _mediator.Send(new CreateCouponCommand(
                request.Code,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.MinOrderAmount,
                request.MaxDiscountAmount,
                request.UsageLimit,
                request.ExpiresAt,
                request.IsActive,
                request.IsFirstOrderOnly));
            return StatusCode(201, result);
        }

        /// <summary>Updates an existing coupon. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CouponRequest request)
        {
            var result = await _mediator.Send(new UpdateCouponCommand(
                id,
                request.Code,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.MinOrderAmount,
                request.MaxDiscountAmount,
                request.UsageLimit,
                request.ExpiresAt,
                request.IsActive,
                request.IsFirstOrderOnly));
            return Ok(result);
        }

        /// <summary>Deletes a coupon. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCouponCommand(id));
            return Ok(result);
        }

        /// <summary>Toggles a coupon active/inactive. Admin only.</summary>
        [HttpPatch("{id:guid}/toggle")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _mediator.Send(new ToggleCouponActiveCommand(id));
            return Ok(result);
        }

        /// <summary>Validates a coupon code against a given order amount. Public.</summary>
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateCouponRequest request)
        {
            var result = await _mediator.Send(new ValidateCouponQuery(request.Code, request.OrderAmount));
            return Ok(result);
        }
    }
}
