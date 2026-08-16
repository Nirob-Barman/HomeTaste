namespace HomeTaste.Domain.Entities.Payment
{
    public class PaymentGateway : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Provider { get; private set; } = string.Empty; // family key: "stripe" | "bkash"
        public string Slug { get; private set; } = string.Empty;     // variant slug: "stripe_payment_intents" | "bkash_manual" | "bkash_checkout"
        public string Config { get; private set; } = "{}";           // encrypted JSON of field key→value pairs
        public bool IsActive { get; private set; } = true;
        public bool IsSandbox { get; private set; } = true;

        private PaymentGateway() { } // EF Core

        public static PaymentGateway Create(string name, string provider, string slug, string encryptedConfig, bool isActive, bool isSandbox, Guid? createdBy)
        {
            return new PaymentGateway
            {
                Name = name,
                Provider = provider,
                Slug = slug,
                Config = encryptedConfig,
                IsActive = isActive,
                IsSandbox = isSandbox,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }

        public void UpdateDetails(string name, bool isActive, bool isSandbox, string encryptedConfig, Guid? updatedBy)
        {
            Name = name;
            IsActive = isActive;
            IsSandbox = isSandbox;
            Config = encryptedConfig;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void ToggleActive(Guid? updatedBy)
        {
            IsActive = !IsActive;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
