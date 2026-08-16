using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.CreatePaymentGateway
{
    public class CreatePaymentGatewayCommand : IRequest<Result<PaymentGatewayResponse>>
    {
        public CreatePaymentGatewayRequest Request { get; set; }

        public CreatePaymentGatewayCommand(CreatePaymentGatewayRequest request)
        {
            Request = request;
        }
    }
}
