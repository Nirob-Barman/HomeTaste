namespace HomeTaste.Application.Features.PaymentGateways
{
    public record PaymentGatewayResponse(
        Guid Id,
        string Name,
        string Provider,
        string Slug,
        bool IsConfigured,
        string? PublishableKeyHint,
        string? MerchantNumber,
        bool IsActive,
        bool IsSandbox,
        DateTime? CreatedAt);
}
