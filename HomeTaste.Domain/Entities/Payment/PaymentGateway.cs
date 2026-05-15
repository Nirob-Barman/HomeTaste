namespace HomeTaste.Domain.Entities.Payment
{
    public class PaymentGateway : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string GatewayType { get; set; } = "card"; // "card" | "manual" | "checkout"
        public string Config { get; set; } = "{}"; // JSON: {"PublishableKey":"...","SecretKey":"...","MerchantNumber":"..."}
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
    }
}
