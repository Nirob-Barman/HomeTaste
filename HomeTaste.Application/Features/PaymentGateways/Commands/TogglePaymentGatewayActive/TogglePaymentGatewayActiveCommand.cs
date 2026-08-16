using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.TogglePaymentGatewayActive
{
    public class TogglePaymentGatewayActiveCommand : IRequest<Result<PaymentGatewayResponse>>
    {
        public Guid Id { get; set; }

        public TogglePaymentGatewayActiveCommand(Guid id)
        {
            Id = id;
        }
    }
}
