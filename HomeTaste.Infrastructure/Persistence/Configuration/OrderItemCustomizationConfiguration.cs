using HomeTaste.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class OrderItemCustomizationConfiguration : IEntityTypeConfiguration<OrderItemCustomization>
    {
        public void Configure(EntityTypeBuilder<OrderItemCustomization> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).HasMaxLength(150);
            builder.Property(c => c.AdditionalPrice).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
