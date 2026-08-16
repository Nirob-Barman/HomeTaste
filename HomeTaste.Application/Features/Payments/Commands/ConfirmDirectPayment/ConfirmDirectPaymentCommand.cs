using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmDirectPayment
{
    public record ConfirmDirectPaymentCommand(Guid OrderId, string? Gateway, string? TransactionRef, string? Notes)
        : IRequest<Result<PaymentTransactionResponse>>;
}
