using System.Text.Json;
using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Payment;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Payment;

namespace HomeTaste.Application.Services.Payment
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly IConfigEncryptor _encryptor;

        public PaymentGatewayService(
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            IConfigEncryptor encryptor)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _encryptor = encryptor;
        }

        public Task<Result<List<GatewayFamilyResponse>>> GetSchemaAsync()
        {
            var schema = GatewayConfigSchema.Families.Select(f => new GatewayFamilyResponse
            {
                Key = f.Key,
                DisplayName = f.DisplayName,
                Variants = f.Variants.Select(v => new GatewayVariantResponse
                {
                    Slug = v.Slug,
                    DisplayName = v.DisplayName,
                    VariantLabel = v.VariantLabel,
                    Fields = v.Fields.Select(fd => new GatewayFieldResponse
                    {
                        Key = fd.Key,
                        Label = fd.Label,
                        IsSecret = fd.IsSecret,
                        IsRequired = fd.IsRequired,
                        Placeholder = fd.Placeholder
                    }).ToList()
                }).ToList()
            }).ToList();

            return Task.FromResult(Result<List<GatewayFamilyResponse>>.Ok(schema, "Schema retrieved.", ResultType.Success));
        }

        public async Task<Result<List<PaymentGatewayResponse>>> GetAllAsync()
        {
            var all = await _unitOfWork.Repository<PaymentGateway>().GetAllAsync();
            var result = all.Select(g => MapToResponse(g)).ToList();
            return Result<List<PaymentGatewayResponse>>.Ok(result, "Gateways retrieved.", ResultType.Success);
        }

        public async Task<Result<List<PaymentGatewayResponse>>> GetActiveAsync()
        {
            var all = await _unitOfWork.Repository<PaymentGateway>().GetAllAsync();
            var result = all.Where(g => g.IsActive).Select(g => MapToResponse(g)).ToList();
            return Result<List<PaymentGatewayResponse>>.Ok(result, "Active gateways retrieved.", ResultType.Success);
        }

        public async Task<Result<PaymentGatewayResponse>> GetByIdAsync(Guid id)
        {
            var gateway = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (gateway == null)
                return Result<PaymentGatewayResponse>.Fail("Gateway not found.", "Not found", ResultType.NotFound);
            return Result<PaymentGatewayResponse>.Ok(MapToResponse(gateway), "Gateway retrieved.", ResultType.Success);
        }

        public async Task<Result<PaymentGatewayResponse>> CreateAsync(CreatePaymentGatewayRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<PaymentGatewayResponse>.Fail("Name is required.", "Validation failed", ResultType.ValidationError);

            var slug = request.Slug.Trim().ToLowerInvariant();

            var variant = GatewayConfigSchema.FindVariant(slug);
            if (variant == null)
                return Result<PaymentGatewayResponse>.Fail($"Unknown gateway slug '{slug}'.", "Validation failed", ResultType.ValidationError);

            var missingField = variant.Fields.FirstOrDefault(f => f.IsRequired && !request.Config.ContainsKey(f.Key));
            if (missingField != null)
                return Result<PaymentGatewayResponse>.Fail($"'{missingField.Label}' is required.", "Validation failed", ResultType.ValidationError);

            var exists = await _unitOfWork.Repository<PaymentGateway>().AnyAsync(g => g.Slug == slug);
            if (exists)
                return Result<PaymentGatewayResponse>.Fail($"A gateway with slug '{slug}' already exists.", "Conflict", ResultType.Conflict);

            var family = GatewayConfigSchema.FindFamily(slug)!;
            Guid.TryParse(_userContextService.UserId, out var userId);

            var configJson = BuildConfigJson(request.Config);
            var entity = new PaymentGateway
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Provider = family.Key,
                Slug = slug,
                Config = _encryptor.Encrypt(configJson),
                IsActive = request.IsActive,
                IsSandbox = request.IsSandbox,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId == Guid.Empty ? null : userId
            };

            await _unitOfWork.Repository<PaymentGateway>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentGatewayResponse>.Ok(MapToResponse(entity), "Gateway created successfully.", ResultType.Created);
        }

        public async Task<Result<PaymentGatewayResponse>> UpdateAsync(Guid id, UpdatePaymentGatewayRequest request)
        {
            var entity = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (entity == null)
                return Result<PaymentGatewayResponse>.Fail("Gateway not found.", "Not found", ResultType.NotFound);

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<PaymentGatewayResponse>.Fail("Name is required.", "Validation failed", ResultType.ValidationError);

            Guid.TryParse(_userContextService.UserId, out var userId);

            entity.Name = request.Name.Trim();
            entity.IsActive = request.IsActive;
            entity.IsSandbox = request.IsSandbox;
            entity.Config = MergeConfig(entity.Config, request.Config);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId == Guid.Empty ? null : userId;

            _unitOfWork.Repository<PaymentGateway>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentGatewayResponse>.Ok(MapToResponse(entity), "Gateway updated successfully.", ResultType.Success);
        }

        public async Task<Result<PaymentGatewayResponse>> ToggleActiveAsync(Guid id)
        {
            var entity = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (entity == null)
                return Result<PaymentGatewayResponse>.Fail("Gateway not found.", "Not found", ResultType.NotFound);

            var config = DecryptConfig(entity.Config);
            var variant = GatewayConfigSchema.FindVariant(entity.Slug);
            if (!entity.IsActive && variant != null)
            {
                var missingField = variant.Fields.FirstOrDefault(f => f.IsRequired && !config.ContainsKey(f.Key));
                if (missingField != null)
                    return Result<PaymentGatewayResponse>.Fail(
                        $"Cannot activate: '{missingField.Label}' is not configured.",
                        "Validation failed", ResultType.ValidationError);
            }

            Guid.TryParse(_userContextService.UserId, out var userId);
            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = userId == Guid.Empty ? null : userId;

            _unitOfWork.Repository<PaymentGateway>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentGatewayResponse>.Ok(MapToResponse(entity),
                $"Gateway is now {(entity.IsActive ? "active" : "inactive")}.", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Repository<PaymentGateway>().GetByIdAsync(id);
            if (entity == null)
                return Result<bool>.Fail("Gateway not found.", "Not found", ResultType.NotFound);

            _unitOfWork.Repository<PaymentGateway>().Remove(entity);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Gateway deleted successfully.", ResultType.Success);
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private Dictionary<string, string> DecryptConfig(string? encryptedJson)
        {
            if (string.IsNullOrWhiteSpace(encryptedJson) || encryptedJson == "{}")
                return [];
            try
            {
                var json = _encryptor.Decrypt(encryptedJson);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
            }
            catch
            {
                // Fallback: treat as plain JSON (legacy unencrypted records)
                return JsonSerializer.Deserialize<Dictionary<string, string>>(encryptedJson) ?? [];
            }
        }

        private static string BuildConfigJson(Dictionary<string, string> incoming)
        {
            var dict = incoming
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value.Trim());
            return JsonSerializer.Serialize(dict);
        }

        // Merges incoming onto existing. Empty string = clear the field; key absent = keep existing.
        private string MergeConfig(string encryptedExisting, Dictionary<string, string> incoming)
        {
            var existing = DecryptConfig(encryptedExisting);
            foreach (var (key, value) in incoming)
            {
                if (string.IsNullOrWhiteSpace(value))
                    existing.Remove(key);
                else
                    existing[key] = value.Trim();
            }
            return _encryptor.Encrypt(JsonSerializer.Serialize(existing));
        }

        private PaymentGatewayResponse MapToResponse(PaymentGateway g)
        {
            var config = DecryptConfig(g.Config);
            config.TryGetValue("publishable_key", out var pubKey);
            config.TryGetValue("app_key", out var appKey);
            config.TryGetValue("merchant_number", out var merchantNum);

            var hintSource = pubKey ?? appKey;

            return new()
            {
                Id = g.Id,
                Name = g.Name,
                Provider = g.Provider,
                Slug = g.Slug,
                IsConfigured = config.Count > 0,
                PublishableKeyHint = hintSource is { Length: > 8 } ? hintSource[..8] + "…" : hintSource,
                MerchantNumber = merchantNum,
                IsActive = g.IsActive,
                IsSandbox = g.IsSandbox,
                CreatedAt = g.CreatedAt
            };
        }
    }
}
