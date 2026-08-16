namespace HomeTaste.Application.Features.PaymentGateways
{
    public record CreatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Dictionary<string, string> Config { get; set; } = [];
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
    }
}
