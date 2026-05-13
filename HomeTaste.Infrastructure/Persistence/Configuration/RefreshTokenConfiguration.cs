using HomeTaste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Token).IsRequired();
            builder.Property(rt => rt.UserId).IsRequired();
            builder.Property(rt => rt.ExpiryDate).IsRequired();
            builder.Property(rt => rt.IsRevoked).HasDefaultValue(false);
            builder.Property(rt => rt.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
