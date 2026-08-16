using FluentValidation;

namespace HomeTaste.Application.Features.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
    {
        public InitiatePaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("OrderId is required.");

            RuleFor(x => x.Gateway).NotEmpty().WithMessage("Gateway is required.");

            RuleFor(x => x.Gateway)
                .MaximumLength(50).WithMessage("Gateway name cannot exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Gateway));

            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
