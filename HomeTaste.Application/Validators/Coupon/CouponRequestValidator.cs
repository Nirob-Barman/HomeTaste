using HomeTaste.Application.DTOs.Coupon;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Validators.Coupon
{
    public static class CouponRequestValidator
    {
        public static List<string> Validate(CouponRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Code))
                errors.Add("Coupon code is required.");
            else if (request.Code.Trim().Length > 50)
                errors.Add("Coupon code cannot exceed 50 characters.");

            if (request.Description?.Length > 500)
                errors.Add("Description cannot exceed 500 characters.");

            if (!Enum.IsDefined(typeof(DiscountType), request.DiscountType))
                errors.Add("Invalid discount type.");

            if (request.DiscountValue <= 0)
                errors.Add("Discount value must be greater than zero.");

            if (request.DiscountType == DiscountType.Percentage && request.DiscountValue > 100)
                errors.Add("Percentage discount cannot exceed 100.");

            if (request.MinOrderAmount.HasValue && request.MinOrderAmount.Value < 0)
                errors.Add("Minimum order amount cannot be negative.");

            if (request.MaxDiscountAmount.HasValue && request.MaxDiscountAmount.Value <= 0)
                errors.Add("Maximum discount amount must be greater than zero.");

            if (request.UsageLimit.HasValue && request.UsageLimit.Value <= 0)
                errors.Add("Usage limit must be greater than zero.");

            if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
                errors.Add("Expiry date must be in the future.");

            return errors;
        }
    }
}
