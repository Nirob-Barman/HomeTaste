using FluentValidation;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Coupons.Commands.UpdateCoupon
{
    public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
    {
        public UpdateCouponCommandValidator()
        {
            RuleFor(x => x.Request.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Coupon code is required.")
                .Must(code => code!.Trim().Length <= 50).WithMessage("Coupon code cannot exceed 50 characters.");

            RuleFor(x => x.Request.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Request.DiscountType)
                .IsInEnum().WithMessage("Invalid discount type.");

            RuleFor(x => x.Request.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than zero.")
                .Must((command, discountValue) => command.Request.DiscountType != DiscountType.Percentage || discountValue <= 100)
                .WithMessage("Percentage discount cannot exceed 100.");

            RuleFor(x => x.Request.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount cannot be negative.")
                .When(x => x.Request.MinOrderAmount.HasValue);

            RuleFor(x => x.Request.MaxDiscountAmount)
                .GreaterThan(0).WithMessage("Maximum discount amount must be greater than zero.")
                .When(x => x.Request.MaxDiscountAmount.HasValue);

            RuleFor(x => x.Request.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than zero.")
                .When(x => x.Request.UsageLimit.HasValue);

            RuleFor(x => x.Request.ExpiresAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.")
                .When(x => x.Request.ExpiresAt.HasValue);
        }
    }
}
