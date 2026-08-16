using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.UpdatePaymentGateway
{
    public record UpdatePaymentGatewayCommand(
        Guid Id,
        string Name,
        Dictionary<string, string> Config,
        bool IsActive,
        bool IsSandbox) : IRequest<Result<PaymentGatewayResponse>>;
}
