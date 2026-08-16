using FluentValidation;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.CreatePaymentGateway
{
    public class CreatePaymentGatewayCommandValidator : AbstractValidator<CreatePaymentGatewayCommand>
    {
        public CreatePaymentGatewayCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
