using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.DeletePaymentGateway
{
    public class DeletePaymentGatewayCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeletePaymentGatewayCommand(Guid id)
        {
            Id = id;
        }
    }
}
