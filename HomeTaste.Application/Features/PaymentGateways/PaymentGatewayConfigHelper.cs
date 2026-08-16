using System.Text.Json;
using HomeTaste.Application.Interfaces;

namespace HomeTaste.Application.Features.PaymentGateways
{
    public static class PaymentGatewayConfigHelper
    {
        public static Dictionary<string, string> DecryptConfig(IConfigEncryptor encryptor, string? encryptedJson)
        {
            if (string.IsNullOrWhiteSpace(encryptedJson) || encryptedJson == "{}")
                return [];
            try
            {
                var json = encryptor.Decrypt(encryptedJson);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
            }
            catch
            {
                // Fallback: treat as plain JSON (legacy unencrypted records)
                return JsonSerializer.Deserialize<Dictionary<string, string>>(encryptedJson) ?? [];
            }
        }

        public static string BuildConfigJson(Dictionary<string, string> incoming)
        {
            var dict = incoming
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value.Trim());
            return JsonSerializer.Serialize(dict);
        }

        // Merges incoming onto existing. Empty string = clear the field; key absent = keep existing.
        public static string MergeConfig(IConfigEncryptor encryptor, string encryptedExisting, Dictionary<string, string> incoming)
        {
            var existing = DecryptConfig(encryptor, encryptedExisting);
            foreach (var (key, value) in incoming)
            {
                if (string.IsNullOrWhiteSpace(value))
                    existing.Remove(key);
                else
                    existing[key] = value.Trim();
            }
            return encryptor.Encrypt(JsonSerializer.Serialize(existing));
        }

        public static PaymentGatewayResponse ToResponse(IConfigEncryptor encryptor, HomeTaste.Domain.Entities.Payment.PaymentGateway g)
        {
            var config = DecryptConfig(encryptor, g.Config);
            config.TryGetValue("publishable_key", out var pubKey);
            config.TryGetValue("app_key", out var appKey);
            config.TryGetValue("merchant_number", out var merchantNum);

            var hintSource = pubKey ?? appKey;

            return new PaymentGatewayResponse(
                g.Id,
                g.Name,
                g.Provider,
                g.Slug,
                config.Count > 0,
                hintSource is { Length: > 8 } ? hintSource[..8] + "…" : hintSource,
                merchantNum,
                g.IsActive,
                g.IsSandbox,
                g.CreatedAt);
        }
    }
}
