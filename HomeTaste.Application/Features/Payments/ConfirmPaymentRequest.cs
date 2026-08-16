namespace HomeTaste.Application.Features.Payments
{
    public record ConfirmPaymentRequest
    {
        public string? TransactionRef { get; set; }
        public string? Notes { get; set; }
    }
}
