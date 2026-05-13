using HomeTaste.Domain.Entities.Coupon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
            builder.HasIndex(c => c.Code).IsUnique();

            builder.Property(c => c.Description).HasMaxLength(500);

            builder.Property(c => c.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
        }
    }
}
