using HomeTaste.Domain.Entities.Loyalty;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
    {
        public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Description).HasMaxLength(500);

            builder.HasIndex(t => t.LoyaltyAccountId);
            builder.HasIndex(t => t.ReferenceId);
        }
    }
}
