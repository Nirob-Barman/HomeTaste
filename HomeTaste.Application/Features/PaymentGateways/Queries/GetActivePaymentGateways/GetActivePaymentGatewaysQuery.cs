using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetActivePaymentGateways
{
    public class GetActivePaymentGatewaysQuery : IRequest<Result<List<PaymentGatewayResponse>>>
    {
    }
}
