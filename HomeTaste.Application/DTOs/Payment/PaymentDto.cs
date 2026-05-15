using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Payment
{
    public class InitiatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public string? Gateway { get; set; }
        public string? Notes { get; set; }
    }

    public class ConfirmPaymentRequest
    {
        public string? TransactionRef { get; set; }
        public string? Notes { get; set; }
    }

    public class RefundPaymentRequest
    {
        public string? Notes { get; set; }
    }

    public class PaymentTransactionResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public string? Gateway { get; set; }
        public string? TransactionRef { get; set; }
        public string? Notes { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ClientSecret { get; set; }
        public string? PublishableKey { get; set; }
        public string? MerchantNumber { get; set; }
    }
}
