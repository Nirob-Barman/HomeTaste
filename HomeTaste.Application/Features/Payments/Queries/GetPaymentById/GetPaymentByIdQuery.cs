using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Queries.GetPaymentById
{
    public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<PaymentTransactionResponse>>;
}
