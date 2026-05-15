namespace HomeTaste.Application.DTOs.Payment
{
    public class PaymentGatewayResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string GatewayType { get; set; } = "card";
        public bool IsConfigured { get; set; }
        public string? PublishableKeyHint { get; set; }  // first 8 chars + "…", admin display only
        public string? MerchantNumber { get; set; }      // customer-safe (bKash phone number)
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string GatewayType { get; set; } = "card";
        public string? PublishableKey { get; set; }
        public string? SecretKey { get; set; }
        public string? MerchantNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
    }

    public class UpdatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? PublishableKey { get; set; }
        public string? SecretKey { get; set; }
        public string? MerchantNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
    }
}
