using FluentValidation;

namespace HomeTaste.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.AddressId).NotEqual(Guid.Empty).WithMessage("A delivery address is required.");

            RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.MealId).NotEqual(Guid.Empty).WithMessage("MealId is required.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
                item.RuleFor(i => i.Quantity).LessThanOrEqualTo(50).WithMessage("Quantity cannot exceed 50.");
                item.RuleFor(i => i.SpecialInstructions).MaximumLength(500).WithMessage("Special instructions cannot exceed 500 characters.");
            });

            RuleFor(x => x.PointsToRedeem).GreaterThanOrEqualTo(0).WithMessage("Points to redeem cannot be negative.");

            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
