using HomeTaste.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class DeliveryPersonnelConfiguration : IEntityTypeConfiguration<DeliveryPersonnel>
    {
        public void Configure(EntityTypeBuilder<DeliveryPersonnel> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName).HasMaxLength(150);
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.VehicleType).HasMaxLength(50);
            builder.Property(p => p.VehicleNumber).HasMaxLength(50);
            builder.Property(p => p.UserId).HasMaxLength(450);

            builder.Property(p => p.Rating)
                .HasColumnType("decimal(3,2)");

            builder.HasMany(p => p.Assignments)
                .WithOne(a => a.DeliveryPersonnel)
                .HasForeignKey(a => a.DeliveryPersonnelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.UserId);
        }
    }
}
