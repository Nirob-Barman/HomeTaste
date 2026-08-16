namespace HomeTaste.Domain.Entities.Payment
{
    public class PaymentGateway : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty; // family key: "stripe" | "bkash"
        public string Slug { get; set; } = string.Empty;     // variant slug: "stripe_payment_intents" | "bkash_manual" | "bkash_checkout"
        public string Config { get; set; } = "{}";           // encrypted JSON of field key→value pairs
        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;
    }
}
