using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.RefundPayment
{
    public record RefundPaymentCommand(Guid TransactionId, string? Notes) : IRequest<Result<PaymentTransactionResponse>>;
}
