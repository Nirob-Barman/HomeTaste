using FluentValidation;

namespace HomeTaste.Application.Features.Coupons.Queries.ValidateCoupon
{
    public class ValidateCouponQueryValidator : AbstractValidator<ValidateCouponQuery>
    {
        public ValidateCouponQueryValidator()
        {
            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("Coupon code is required.");

            RuleFor(x => x.Request.OrderAmount)
                .GreaterThan(0).WithMessage("Order amount must be greater than zero.");
        }
    }
}
