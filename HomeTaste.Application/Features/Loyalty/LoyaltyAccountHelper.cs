using HomeTaste.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using LoyaltyAccountEntity = HomeTaste.Domain.Entities.Loyalty.LoyaltyAccount;

namespace HomeTaste.Application.Features.Loyalty
{
    public static class LoyaltyAccountHelper
    {
        private const int PointsRedemptionRate = 100; // 100 points = $1

        public static async Task<LoyaltyAccountEntity> GetOrCreateAccountAsync(IApplicationDbContext context, string userId, CancellationToken cancellationToken)
        {
            var account = await context.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
            if (account != null) return account;

            account = LoyaltyAccountEntity.Create(userId);
            context.LoyaltyAccounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);

            return account;
        }

        private static int PointsToNextTier(int totalPointsEarned)
        {
            int[] thresholds = [1_000, 5_000, 10_000];
            foreach (var t in thresholds)
                if (totalPointsEarned < t) return t - totalPointsEarned;
            return 0;
        }

        public static LoyaltyAccountResponse ToResponse(LoyaltyAccountEntity a) => new(
            a.Id,
            a.UserId,
            a.CurrentPoints,
            a.TotalPointsEarned,
            a.Tier,
            a.Tier.ToString(),
            PointsToNextTier(a.TotalPointsEarned),
            Math.Round((decimal)a.CurrentPoints / PointsRedemptionRate, 2),
            a.CreatedAt);
    }
}
