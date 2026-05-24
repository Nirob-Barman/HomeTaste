namespace HomeTaste.Application.DTOs.Payment
{
    // ─── Gateway CRUD DTOs ────────────────────────────────────────────────────

    public class PaymentGatewayResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsConfigured { get; set; }
        public string? PublishableKeyHint { get; set; }  // first 8 chars of publishable_key/app_key + "…"
        public string? MerchantNumber { get; set; }      // customer-safe (bKash merchant number)
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Dictionary<string, string> Config { get; set; } = [];
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
    }

    public class UpdatePaymentGatewayRequest
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string> Config { get; set; } = [];
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
    }

    // ─── Schema DTOs (returned by GET /api/paymentgateway/schema) ────────────

    public class GatewayFieldResponse
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsSecret { get; set; }
        public bool IsRequired { get; set; }
        public string? Placeholder { get; set; }
    }

    public class GatewayVariantResponse
    {
        public string Slug { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string VariantLabel { get; set; } = string.Empty;
        public List<GatewayFieldResponse> Fields { get; set; } = [];
    }

    public class GatewayFamilyResponse
    {
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<GatewayVariantResponse> Variants { get; set; } = [];
    }
}
