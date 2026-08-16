using HomeTaste.Domain.Entities.Payment;

namespace HomeTaste.Application.Features.Payments
{
    public static class PaymentMapper
    {
        public static PaymentTransactionResponse ToResponse(PaymentTransaction t) => new()
        {
            Id = t.Id,
            OrderId = t.OrderId,
            Amount = t.Amount,
            Status = t.Status,
            StatusLabel = t.Status.ToString(),
            Gateway = t.Gateway,
            TransactionRef = t.TransactionRef,
            Notes = t.Notes,
            PaidAt = t.PaidAt,
            RefundedAt = t.RefundedAt,
            CreatedAt = t.CreatedAt
        };
    }
}
