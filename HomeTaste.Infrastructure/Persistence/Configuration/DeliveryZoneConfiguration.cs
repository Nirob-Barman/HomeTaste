using HomeTaste.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
    {
        public void Configure(EntityTypeBuilder<DeliveryZone> builder)
        {
            builder.HasKey(z => z.Id);

            builder.Property(z => z.Name).IsRequired().HasMaxLength(100);
            builder.Property(z => z.Description).HasMaxLength(500);

            var stringListComparer = new ValueComparer<List<string>>(
                (a, b) => a != null && b != null && a.SequenceEqual(b),
                v => v.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                v => v.ToList());

            builder.Property(z => z.AllowedCities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(stringListComparer);

            builder.Property(z => z.AllowedPostalCodes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(stringListComparer);

            builder.HasIndex(z => z.IsActive);
        }
    }
}
