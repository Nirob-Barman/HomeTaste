using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Coupon;
using HomeTaste.Application.Interfaces.Coupon;
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
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>Gets all coupons with optional search and pagination. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null!)
        {
            var result = await _couponService.GetAllCouponsAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a coupon by ID. Admin only.</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _couponService.GetCouponByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Creates a new coupon. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CouponRequest request)
        {
            var result = await _couponService.CreateCouponAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates an existing coupon. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] CouponRequest request)
        {
            var result = await _couponService.UpdateCouponAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes a coupon. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Toggles a coupon active/inactive. Admin only.</summary>
        [HttpPatch("{id:guid}/toggle")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _couponService.ToggleActiveAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Validates a coupon code against a given order amount. Public.</summary>
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateCouponRequest request)
        {
            var result = await _couponService.ValidateCouponAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
