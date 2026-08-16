using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Queries.GetPaymentByOrderId
{
    public record GetPaymentByOrderIdQuery(Guid OrderId) : IRequest<Result<PaymentTransactionResponse>>;
}
