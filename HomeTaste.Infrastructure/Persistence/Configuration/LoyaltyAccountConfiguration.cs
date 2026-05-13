using HomeTaste.Domain.Entities.Loyalty;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
    {
        public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId).HasMaxLength(450);
            builder.HasIndex(a => a.UserId).IsUnique();

            builder.HasMany(a => a.Transactions)
                .WithOne(t => t.LoyaltyAccount)
                .HasForeignKey(t => t.LoyaltyAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
