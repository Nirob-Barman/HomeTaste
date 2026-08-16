using HomeTaste.Application.Interfaces.Loyalty;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using LoyaltyAccountEntity = HomeTaste.Domain.Entities.Loyalty.LoyaltyAccount;
using LoyaltyTransactionEntity = HomeTaste.Domain.Entities.Loyalty.LoyaltyTransaction;

namespace HomeTaste.Application.Services.Loyalty
{
    public class LoyaltyPointsService : ILoyaltyPointsService
    {
        private const int PointsPerDollar = 1;

        private readonly IApplicationDbContext _context;

        public LoyaltyPointsService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task EarnPointsAsync(string userId, Guid orderId, decimal orderTotal)
        {
            var pointsEarned = (int)Math.Floor(orderTotal) * PointsPerDollar;
            if (pointsEarned <= 0) return;

            var account = await _context.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
            if (account == null)
            {
                account = LoyaltyAccountEntity.Create(userId);
                _context.LoyaltyAccounts.Add(account);
                await _context.SaveChangesAsync();
            }

            account.AdjustPoints(pointsEarned);

            var transaction = LoyaltyTransactionEntity.Create(
                account.Id,
                pointsEarned,
                LoyaltyTransactionType.Earned,
                orderId,
                $"Earned {pointsEarned} point{(pointsEarned == 1 ? "" : "s")} for order payment.");

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
