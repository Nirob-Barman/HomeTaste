using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.DeletePaymentGateway
{
    public record DeletePaymentGatewayCommand(Guid Id) : IRequest<Result<bool>>;
}
