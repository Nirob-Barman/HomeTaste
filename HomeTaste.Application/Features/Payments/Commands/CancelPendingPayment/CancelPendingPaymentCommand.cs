using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.CancelPendingPayment
{
    public record CancelPendingPaymentCommand(Guid TransactionId) : IRequest<Result<PaymentTransactionResponse>>;
}
