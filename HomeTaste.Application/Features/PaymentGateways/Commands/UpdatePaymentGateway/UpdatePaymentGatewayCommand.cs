using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.UpdatePaymentGateway
{
    public class UpdatePaymentGatewayCommand : IRequest<Result<PaymentGatewayResponse>>
    {
        public Guid Id { get; set; }
        public UpdatePaymentGatewayRequest Request { get; set; }

        public UpdatePaymentGatewayCommand(Guid id, UpdatePaymentGatewayRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
