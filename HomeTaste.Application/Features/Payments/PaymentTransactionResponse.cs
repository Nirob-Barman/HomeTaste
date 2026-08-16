using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Payments
{
    public record PaymentTransactionResponse
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
        public string? RedirectUrl { get; set; }
        public string? ClientSecret { get; set; }
        public string? PublishableKey { get; set; }
        public string? MerchantNumber { get; set; }
    }
}
