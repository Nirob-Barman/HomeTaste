namespace HomeTaste.Application.Features.Payments
{
    public record InitiatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public string? Gateway { get; set; }
        public string? Notes { get; set; }
    }
}
