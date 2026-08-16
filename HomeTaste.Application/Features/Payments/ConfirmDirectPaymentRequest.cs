namespace HomeTaste.Application.Features.Payments
{
    public record ConfirmDirectPaymentRequest
    {
        public Guid OrderId { get; set; }
        public string? Gateway { get; set; }
        public string? TransactionRef { get; set; }
        public string? Notes { get; set; }
    }
}
