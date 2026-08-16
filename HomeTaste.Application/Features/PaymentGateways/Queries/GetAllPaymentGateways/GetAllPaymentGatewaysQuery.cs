using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetAllPaymentGateways
{
    public record GetAllPaymentGatewaysQuery : IRequest<Result<List<PaymentGatewayResponse>>>;
}
