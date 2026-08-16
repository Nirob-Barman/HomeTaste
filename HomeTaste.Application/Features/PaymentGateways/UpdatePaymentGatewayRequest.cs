namespace HomeTaste.Application.Features.PaymentGateways
{
    public record UpdatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string> Config { get; set; } = [];
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
    }
}
