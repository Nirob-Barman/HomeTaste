using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Loyalty
{
    public class LoyaltyAccount : BaseEntity
    {
        private static readonly (int MinPoints, LoyaltyTier Tier)[] TierThresholds =
        [
            (10_000, LoyaltyTier.Platinum),
            (5_000, LoyaltyTier.Gold),
            (1_000, LoyaltyTier.Silver),
            (0,     LoyaltyTier.Bronze)
        ];

        public string? UserId { get; private set; }
        public int CurrentPoints { get; private set; }
        public int TotalPointsEarned { get; private set; }
        public LoyaltyTier Tier { get; private set; } = LoyaltyTier.Bronze;

        public List<LoyaltyTransaction>? Transactions { get; set; }

        private LoyaltyAccount() { } // EF Core

        public static LoyaltyAccount Create(string userId)
        {
            return new LoyaltyAccount
            {
                UserId = userId,
                CurrentPoints = 0,
                TotalPointsEarned = 0,
                Tier = LoyaltyTier.Bronze,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Used for both earning points (always positive) and admin adjustments (positive or negative).
        public void AdjustPoints(int points)
        {
            CurrentPoints += points;
            if (points > 0)
                TotalPointsEarned += points;

            Tier = CalculateTier(TotalPointsEarned);
            UpdatedAt = DateTime.UtcNow;
        }

        // Redemption deduction only — does not affect TotalPointsEarned or Tier.
        public void DeductPoints(int points)
        {
            CurrentPoints -= points;
            UpdatedAt = DateTime.UtcNow;
        }

        private static LoyaltyTier CalculateTier(int totalPointsEarned)
        {
            foreach (var (min, tier) in TierThresholds)
                if (totalPointsEarned >= min) return tier;
            return LoyaltyTier.Bronze;
        }
    }
}
