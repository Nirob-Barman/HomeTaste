using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.InitiatePayment
{
    public record InitiatePaymentCommand(Guid OrderId, string? Gateway, string? Notes, string CallbackBaseUrl)
        : IRequest<Result<PaymentTransactionResponse>>;
}
