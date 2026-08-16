using FluentValidation;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.UpdatePaymentGateway
{
    public class UpdatePaymentGatewayCommandValidator : AbstractValidator<UpdatePaymentGatewayCommand>
    {
        public UpdatePaymentGatewayCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
