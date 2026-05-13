using HomeTaste.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(i => i.SpecialInstructions).HasMaxLength(500);

            builder.HasOne(i => i.Meal)
                .WithMany()
                .HasForeignKey(i => i.MealId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.Customizations)
                .WithOne(c => c.OrderItem)
                .HasForeignKey(c => c.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
