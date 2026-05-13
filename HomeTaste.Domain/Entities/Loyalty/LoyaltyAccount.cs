using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Loyalty
{
    public class LoyaltyAccount : BaseEntity
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public int CurrentPoints { get; set; }
        public int TotalPointsEarned { get; set; }
        public LoyaltyTier Tier { get; set; } = LoyaltyTier.Bronze;

        public List<LoyaltyTransaction>? Transactions { get; set; }
    }
}
