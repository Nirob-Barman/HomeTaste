using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmPayment
{
    public record ConfirmPaymentCommand(Guid TransactionId, string? TransactionRef, string? Notes)
        : IRequest<Result<PaymentTransactionResponse>>;
}
