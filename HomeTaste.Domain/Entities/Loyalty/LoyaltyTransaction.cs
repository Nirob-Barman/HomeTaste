using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Loyalty
{
    public class LoyaltyTransaction : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LoyaltyAccountId { get; set; }
        public int Points { get; set; }
        public LoyaltyTransactionType TransactionType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Description { get; set; }

        public LoyaltyAccount? LoyaltyAccount { get; set; }
    }
}
