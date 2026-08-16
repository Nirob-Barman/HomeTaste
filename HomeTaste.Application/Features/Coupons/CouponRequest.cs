using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Coupons
{
    public class CouponRequest
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFirstOrderOnly { get; set; }
    }
}
