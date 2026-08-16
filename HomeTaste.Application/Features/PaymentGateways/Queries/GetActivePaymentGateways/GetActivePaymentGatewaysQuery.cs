using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetActivePaymentGateways
{
    public record GetActivePaymentGatewaysQuery : IRequest<Result<List<PaymentGatewayResponse>>>;
}
