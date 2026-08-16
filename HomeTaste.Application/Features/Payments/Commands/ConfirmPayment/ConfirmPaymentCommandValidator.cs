using FluentValidation;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmPayment
{
    public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
    {
        public ConfirmPaymentCommandValidator()
        {
            RuleFor(x => x.TransactionRef).MaximumLength(200).WithMessage("Transaction reference cannot exceed 200 characters.");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
