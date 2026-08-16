using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.CreatePaymentGateway
{
    public record CreatePaymentGatewayCommand(
        string Name,
        string Slug,
        Dictionary<string, string> Config,
        bool IsActive,
        bool IsSandbox) : IRequest<Result<PaymentGatewayResponse>>;
}
