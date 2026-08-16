namespace HomeTaste.Application.Features.PaymentGateways
{
    public record GatewayFieldResponse(
        string Key,
        string Label,
        bool IsSecret,
        bool IsRequired,
        string? Placeholder);

    public record GatewayVariantResponse(
        string Slug,
        string DisplayName,
        string VariantLabel,
        List<GatewayFieldResponse> Fields);

    public record GatewayFamilyResponse(
        string Key,
        string DisplayName,
        List<GatewayVariantResponse> Variants);
}
