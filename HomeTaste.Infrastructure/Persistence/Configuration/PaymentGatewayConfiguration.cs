using HomeTaste.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class PaymentGatewayConfiguration : IEntityTypeConfiguration<PaymentGateway>
    {
        public void Configure(EntityTypeBuilder<PaymentGateway> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
            builder.Property(g => g.Slug).HasMaxLength(50).IsRequired();
            builder.HasIndex(g => g.Slug).IsUnique();

            builder.Property(g => g.GatewayType).HasMaxLength(50).IsRequired().HasDefaultValue("card");
            builder.Property(g => g.Config).HasColumnType("nvarchar(max)").IsRequired().HasDefaultValue("{}");

            builder.HasIndex(g => g.IsActive);
        }
    }
}
