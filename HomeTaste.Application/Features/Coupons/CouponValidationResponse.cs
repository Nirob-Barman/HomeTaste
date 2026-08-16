namespace HomeTaste.Application.Features.Coupons
{
    public class CouponValidationResponse
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Message { get; set; }
        public CouponResponse? Coupon { get; set; }
    }
}
