using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Coupon
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

    public class CouponResponse
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirstOrderOnly { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ValidateCouponRequest
    {
        public string? Code { get; set; }
        public decimal OrderAmount { get; set; }
    }

    public class CouponValidationResponse
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Message { get; set; }
        public CouponResponse? Coupon { get; set; }
    }
}
