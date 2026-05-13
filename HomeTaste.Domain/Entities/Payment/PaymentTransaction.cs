using HomeTaste.Domain.Enums;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Domain.Entities.Payment
{
    public class PaymentTransaction : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? Gateway { get; set; }
        public string? TransactionRef { get; set; }
        public string? Notes { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        public OrderEntity? Order { get; set; }
    }
}
