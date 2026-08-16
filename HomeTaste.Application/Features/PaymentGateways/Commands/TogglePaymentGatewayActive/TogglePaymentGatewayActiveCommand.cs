using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.TogglePaymentGatewayActive
{
    public record TogglePaymentGatewayActiveCommand(Guid Id) : IRequest<Result<PaymentGatewayResponse>>;
}
