using FluentValidation;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid order status.");

            RuleFor(x => x.CancellationReason)
                .NotEmpty().WithMessage("Cancellation reason is required when cancelling an order.")
                .When(x => x.Status == OrderStatus.Cancelled);

            RuleFor(x => x.CancellationReason).MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.");
        }
    }
}
