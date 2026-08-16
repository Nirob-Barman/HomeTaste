namespace HomeTaste.Application.Features.Coupons
{
    public static class CouponMapper
    {
        public static CouponResponse ToResponse(HomeTaste.Domain.Entities.Coupon.Coupon coupon) => new()
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Description = coupon.Description,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            MinOrderAmount = coupon.MinOrderAmount,
            MaxDiscountAmount = coupon.MaxDiscountAmount,
            UsageLimit = coupon.UsageLimit,
            UsageCount = coupon.UsageCount,
            ExpiresAt = coupon.ExpiresAt,
            IsActive = coupon.IsActive,
            IsFirstOrderOnly = coupon.IsFirstOrderOnly,
            CreatedAt = coupon.CreatedAt
        };
    }
}
