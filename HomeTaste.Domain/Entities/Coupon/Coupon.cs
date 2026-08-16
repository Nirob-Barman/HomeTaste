using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Coupon
{
    public class Coupon : BaseEntity
    {
        public string? Code { get; private set; }
        public string? Description { get; private set; }
        public DiscountType DiscountType { get; private set; }
        public decimal DiscountValue { get; private set; }
        public decimal? MinOrderAmount { get; private set; }
        public decimal? MaxDiscountAmount { get; private set; }
        public int? UsageLimit { get; private set; }
        public int UsageCount { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsFirstOrderOnly { get; private set; }

        private Coupon() { } // EF Core

        public static Coupon Create(
            string? code,
            string? description,
            DiscountType discountType,
            decimal discountValue,
            decimal? minOrderAmount,
            decimal? maxDiscountAmount,
            int? usageLimit,
            DateTime? expiresAt,
            bool isActive,
            bool isFirstOrderOnly)
        {
            return new Coupon
            {
                Code = code,
                Description = description,
                DiscountType = discountType,
                DiscountValue = discountValue,
                MinOrderAmount = minOrderAmount,
                MaxDiscountAmount = maxDiscountAmount,
                UsageLimit = usageLimit,
                UsageCount = 0,
                ExpiresAt = expiresAt,
                IsActive = isActive,
                IsFirstOrderOnly = isFirstOrderOnly,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(
            string? code,
            string? description,
            DiscountType discountType,
            decimal discountValue,
            decimal? minOrderAmount,
            decimal? maxDiscountAmount,
            int? usageLimit,
            DateTime? expiresAt,
            bool isActive,
            bool isFirstOrderOnly)
        {
            Code = code;
            Description = description;
            DiscountType = discountType;
            DiscountValue = discountValue;
            MinOrderAmount = minOrderAmount;
            MaxDiscountAmount = maxDiscountAmount;
            UsageLimit = usageLimit;
            ExpiresAt = expiresAt;
            IsActive = isActive;
            IsFirstOrderOnly = isFirstOrderOnly;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleActive()
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementUsageCount()
        {
            UsageCount++;
        }
    }
}
