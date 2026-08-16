using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Loyalty
{
    public record LoyaltyAccountResponse(
        Guid Id,
        string? UserId,
        int CurrentPoints,
        int TotalPointsEarned,
        LoyaltyTier Tier,
        string TierLabel,
        int PointsToNextTier,
        decimal PointsValueInCurrency,
        DateTime? CreatedAt);
}
