using FluentValidation;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmDirectPayment
{
    public class ConfirmDirectPaymentCommandValidator : AbstractValidator<ConfirmDirectPaymentCommand>
    {
        public ConfirmDirectPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("Order ID is required.");
            RuleFor(x => x.Gateway).NotEmpty().WithMessage("Gateway is required.");
            RuleFor(x => x.TransactionRef).MaximumLength(200).WithMessage("Transaction reference cannot exceed 200 characters.");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
