namespace HomeTaste.Application.Payment
{
    public class GatewayFieldDefinition
    {
        public string Key { get; init; } = string.Empty;
        public string Label { get; init; } = string.Empty;
        public bool IsSecret { get; init; }
        public bool IsRequired { get; init; }
        public string? Placeholder { get; init; }
    }

    public class GatewayVariant
    {
        public string Slug { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string VariantLabel { get; init; } = string.Empty;
        public List<GatewayFieldDefinition> Fields { get; init; } = [];
    }

    public class GatewayFamily
    {
        public string Key { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public List<GatewayVariant> Variants { get; init; } = [];
    }

    public static class GatewayConfigSchema
    {
        public static readonly List<GatewayFamily> Families =
        [
            new GatewayFamily
            {
                Key = "stripe",
                DisplayName = "Stripe",
                Variants =
                [
                    new GatewayVariant
                    {
                        Slug = "stripe_payment_intents",
                        DisplayName = "Stripe Payment Intents",
                        VariantLabel = "Payment Intents",
                        Fields =
                        [
                            new GatewayFieldDefinition { Key = "secret_key",      Label = "Secret Key",       IsSecret = true,  IsRequired = true,  Placeholder = "sk_test_..." },
                            new GatewayFieldDefinition { Key = "publishable_key", Label = "Publishable Key",  IsSecret = false, IsRequired = true,  Placeholder = "pk_test_..." },
                            new GatewayFieldDefinition { Key = "webhook_secret",  Label = "Webhook Secret",   IsSecret = true,  IsRequired = false, Placeholder = "whsec_..." },
                        ]
                    }
                ]
            },

            new GatewayFamily
            {
                Key = "bkash",
                DisplayName = "bKash",
                Variants =
                [
                    new GatewayVariant
                    {
                        Slug = "bkash_manual",
                        DisplayName = "bKash Manual",
                        VariantLabel = "Manual (Transaction ID)",
                        Fields =
                        [
                            new GatewayFieldDefinition { Key = "merchant_number", Label = "Merchant Number", IsSecret = false, IsRequired = true, Placeholder = "e.g. 01XXXXXXXXX" },
                        ]
                    },
                    new GatewayVariant
                    {
                        Slug = "bkash_checkout",
                        DisplayName = "bKash Checkout",
                        VariantLabel = "Checkout (API)",
                        Fields =
                        [
                            new GatewayFieldDefinition { Key = "app_key",     Label = "App Key",     IsSecret = false, IsRequired = true, Placeholder = "bKash App Key" },
                            new GatewayFieldDefinition { Key = "app_secret",  Label = "App Secret",  IsSecret = true,  IsRequired = true, Placeholder = "bKash App Secret" },
                            new GatewayFieldDefinition { Key = "username",    Label = "Username",    IsSecret = false, IsRequired = true, Placeholder = "bKash API Username" },
                            new GatewayFieldDefinition { Key = "password",    Label = "Password",    IsSecret = true,  IsRequired = true, Placeholder = "bKash API Password" },
                        ]
                    }
                ]
            }
        ];

        public static GatewayVariant? FindVariant(string slug) =>
            Families.SelectMany(f => f.Variants).FirstOrDefault(v => v.Slug == slug);

        public static GatewayFamily? FindFamily(string slug) =>
            Families.FirstOrDefault(f => f.Variants.Any(v => v.Slug == slug));
    }
}
