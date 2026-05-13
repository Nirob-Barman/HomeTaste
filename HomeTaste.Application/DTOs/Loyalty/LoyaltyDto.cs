using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Loyalty
{
    public class RedeemPointsRequest
    {
        public int PointsToRedeem { get; set; }
    }

    public class AdjustPointsRequest
    {
        public string? UserId { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
    }

    public class LoyaltyAccountResponse
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public int CurrentPoints { get; set; }
        public int TotalPointsEarned { get; set; }
        public LoyaltyTier Tier { get; set; }
        public string? TierLabel { get; set; }
        public int PointsToNextTier { get; set; }
        public decimal PointsValueInCurrency { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class LoyaltyTransactionResponse
    {
        public Guid Id { get; set; }
        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; }
        public string? TypeLabel { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PointsPreviewResponse
    {
        public int PointsToRedeem { get; set; }
        public decimal DiscountAmount { get; set; }
        public int RemainingPoints { get; set; }
    }
}
