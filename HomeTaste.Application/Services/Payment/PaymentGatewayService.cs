using System.Text.Json;
using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Payment;

namespace HomeTaste.Application.Services.Payment
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public PaymentGatewayService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<List<PaymentGatewayResponse>>> GetAllAsync()
        {
            var all = await _unitOfWork.Repository<PaymentGateway>().GetAllAsync();
            var result = all.Select(MapToResponse).ToList();
            return Result<List<PaymentGatewayResponse>>.Ok(result, "Gateways retrieved.", ResultType.Success);
        }

        public async Task<Result<List<PaymentGatewayResponse>>> GetActiveAsync()
        {
            var all = await _unitOfWork.Repository<PaymentGateway>().GetAllAsync();
            var result = all.Where(g => g.IsActive).Select(MapToResponse).ToList();
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
            if (string.IsNullOrWhiteSpace(request.Slug))
                return Result<PaymentGatewayResponse>.Fail("Slug is required.", "Validation failed", ResultType.ValidationError);

            var slug = request.Slug.Trim().ToLowerInvariant();
            var exists = await _unitOfWork.Repository<PaymentGateway>().AnyAsync(g => g.Slug == slug);
            if (exists)
                return Result<PaymentGatewayResponse>.Fail($"A gateway with slug '{slug}' already exists.", "Conflict", ResultType.Conflict);

            Guid.TryParse(_userContextService.UserId, out var userId);

            var entity = new PaymentGateway
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Slug = slug,
                GatewayType = string.IsNullOrWhiteSpace(request.GatewayType) ? "card" : request.GatewayType.Trim().ToLowerInvariant(),
                Config = BuildConfig(request.PublishableKey, request.SecretKey, request.MerchantNumber),
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
            entity.Config = MergeConfig(entity.Config, request.PublishableKey, request.SecretKey, request.MerchantNumber);
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

        private static Dictionary<string, string> ParseConfig(string? json) =>
            JsonSerializer.Deserialize<Dictionary<string, string>>(json ?? "{}") ?? new();

        private static string BuildConfig(string? publishableKey, string? secretKey, string? merchantNumber)
        {
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(publishableKey)) dict["PublishableKey"] = publishableKey.Trim();
            if (!string.IsNullOrWhiteSpace(secretKey))      dict["SecretKey"]      = secretKey.Trim();
            if (!string.IsNullOrWhiteSpace(merchantNumber)) dict["MerchantNumber"] = merchantNumber.Trim();
            return JsonSerializer.Serialize(dict);
        }

        // Merges incoming values onto existing config. Blank string = clear the field; null = keep existing.
        private static string MergeConfig(string existing, string? publishableKey, string? secretKey, string? merchantNumber)
        {
            var dict = ParseConfig(existing);
            if (!string.IsNullOrWhiteSpace(publishableKey)) dict["PublishableKey"] = publishableKey.Trim();
            if (!string.IsNullOrWhiteSpace(secretKey))      dict["SecretKey"]      = secretKey.Trim();
            if (merchantNumber != null)
            {
                if (string.IsNullOrWhiteSpace(merchantNumber)) dict.Remove("MerchantNumber");
                else dict["MerchantNumber"] = merchantNumber.Trim();
            }
            return JsonSerializer.Serialize(dict);
        }

        private static PaymentGatewayResponse MapToResponse(PaymentGateway g)
        {
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(g.Config ?? "{}") ?? new();
            config.TryGetValue("PublishableKey", out var pubKey);
            config.TryGetValue("MerchantNumber", out var merchantNum);

            return new()
            {
                Id = g.Id,
                Name = g.Name,
                Slug = g.Slug,
                GatewayType = g.GatewayType,
                IsConfigured = config.Count > 0,
                PublishableKeyHint = pubKey is { Length: > 8 } ? pubKey[..8] + "…" : pubKey,
                MerchantNumber = merchantNum,
                IsActive = g.IsActive,
                IsSandbox = g.IsSandbox,
                CreatedAt = g.CreatedAt
            };
        }
    }
}
