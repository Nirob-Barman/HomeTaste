using HomeTaste.Domain.Entities.Address;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Label).HasMaxLength(50);
            builder.Property(a => a.AddressLine1).HasMaxLength(200);
            builder.Property(a => a.AddressLine2).HasMaxLength(200);
            builder.Property(a => a.City).HasMaxLength(100);
            builder.Property(a => a.State).HasMaxLength(100);
            builder.Property(a => a.PostalCode).HasMaxLength(20);
            builder.Property(a => a.Country).HasMaxLength(100);

            builder.HasIndex(a => a.UserId);
        }
    }
}
