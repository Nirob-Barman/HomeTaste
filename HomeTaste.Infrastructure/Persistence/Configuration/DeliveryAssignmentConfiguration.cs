using HomeTaste.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class DeliveryAssignmentConfiguration : IEntityTypeConfiguration<DeliveryAssignment>
    {
        public void Configure(EntityTypeBuilder<DeliveryAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Notes).HasMaxLength(500);

            builder.HasOne(a => a.Order)
                .WithMany()
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.OrderId);
            builder.HasIndex(a => a.DeliveryPersonnelId);
            builder.HasIndex(a => a.Status);
        }
    }
}
