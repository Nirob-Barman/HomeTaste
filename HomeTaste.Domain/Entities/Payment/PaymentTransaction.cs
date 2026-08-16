using HomeTaste.Domain.Enums;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Domain.Entities.Payment
{
    public class PaymentTransaction : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public string? Gateway { get; private set; }
        public string? TransactionRef { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime? RefundedAt { get; private set; }

        public OrderEntity? Order { get; set; }

        private PaymentTransaction() { } // EF Core

        // Id is pre-assigned upfront by the caller — redirect-flow gateways embed it in the callback URL before this row is persisted.
        public static PaymentTransaction CreatePending(Guid id, Guid orderId, decimal amount, string gateway, string? transactionRef, string? notes)
        {
            return new PaymentTransaction
            {
                Id = id,
                OrderId = orderId,
                Amount = amount,
                Status = PaymentStatus.Pending,
                Gateway = gateway,
                TransactionRef = transactionRef,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static PaymentTransaction CreateSuccessful(Guid orderId, decimal amount, string gateway, string? transactionRef, string? notes)
        {
            return new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Amount = amount,
                Status = PaymentStatus.Success,
                Gateway = gateway,
                TransactionRef = transactionRef,
                Notes = notes,
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void MarkFailed()
        {
            Status = PaymentStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkSuccessful(string? newTransactionRef, string? notes)
        {
            if (newTransactionRef != null)
                TransactionRef = newTransactionRef;

            Status = PaymentStatus.Success;
            Notes = notes ?? Notes;
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkRefunded(string? notes)
        {
            Status = PaymentStatus.Refunded;
            Notes = notes ?? Notes;
            RefundedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
