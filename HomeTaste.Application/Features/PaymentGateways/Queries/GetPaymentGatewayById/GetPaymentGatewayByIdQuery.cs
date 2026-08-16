using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetPaymentGatewayById
{
    public record GetPaymentGatewayByIdQuery(Guid Id) : IRequest<Result<PaymentGatewayResponse>>;
}
