using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Coupons.Queries.ValidateCoupon
{
    public class ValidateCouponQueryHandler : IRequestHandler<ValidateCouponQuery, Result<CouponValidationResponse>>
    {
        private readonly IApplicationDbContext _context;

        public ValidateCouponQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CouponValidationResponse>> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
        {
            var code = request.Request.Code?.Trim().ToUpperInvariant();
            var orderAmount = request.Request.OrderAmount;

            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            if (coupon == null)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon not found." }, "Validation complete");

            if (!coupon.IsActive)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon is inactive." }, "Validation complete");

            if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon has expired." }, "Validation complete");

            if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = "Coupon usage limit reached." }, "Validation complete");

            if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount.Value)
                return Result<CouponValidationResponse>.Ok(new CouponValidationResponse { IsValid = false, Message = $"Minimum order amount of {coupon.MinOrderAmount:C} required." }, "Validation complete");

            var discountAmount = coupon.DiscountType == DiscountType.Percentage
                ? orderAmount * (coupon.DiscountValue / 100m)
                : coupon.DiscountValue;

            if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                discountAmount = coupon.MaxDiscountAmount.Value;

            discountAmount = Math.Min(discountAmount, orderAmount);

            return Result<CouponValidationResponse>.Ok(new CouponValidationResponse
            {
                IsValid = true,
                DiscountAmount = Math.Round(discountAmount, 2),
                Message = "Coupon applied successfully.",
                Coupon = CouponMapper.ToResponse(coupon)
            }, "Validation complete");
        }
    }
}
