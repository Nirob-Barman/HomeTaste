using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Loyalty
{
    public class LoyaltyTransaction : BaseEntity
    {
        public Guid LoyaltyAccountId { get; private set; }
        public int Points { get; private set; }
        public LoyaltyTransactionType TransactionType { get; private set; }
        public Guid? ReferenceId { get; private set; }
        public string? Description { get; private set; }

        public LoyaltyAccount? LoyaltyAccount { get; set; }

        private LoyaltyTransaction() { } // EF Core

        public static LoyaltyTransaction Create(Guid loyaltyAccountId, int points, LoyaltyTransactionType transactionType, Guid? referenceId, string? description)
        {
            return new LoyaltyTransaction
            {
                LoyaltyAccountId = loyaltyAccountId,
                Points = points,
                TransactionType = transactionType,
                ReferenceId = referenceId,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
