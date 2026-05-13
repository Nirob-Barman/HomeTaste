using HomeTaste.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(o => o.LoyaltyDiscountAmount).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(o => o.Notes).HasMaxLength(500);
            builder.Property(o => o.CancellationReason).HasMaxLength(500);

            builder.HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Coupon)
                .WithMany()
                .HasForeignKey(o => o.CouponId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasMany(o => o.OrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
        }
    }
}
