using HomeTaste.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class UnitConfiguration : IEntityTypeConfiguration<Units>
    {
        public void Configure(EntityTypeBuilder<Units> builder)
        {
            // Primary Key Configuration
            builder.HasKey(u => u.Id);

            // Property Configuration
            builder.Property(u => u.Name).IsRequired().HasMaxLength(50);  // Set max length for Name (e.g., Kilogram, Liter)

            builder.Property(u => u.Abbreviation).IsRequired(true)
                .HasMaxLength(10);  // Set max length for Abbreviation (e.g., kg, pcs, l)

            // Optionally, create a unique constraint on the Name and/or Abbreviation if necessary
            builder.HasIndex(u => u.Name)
                .IsUnique();  // Ensure unit names are unique

            builder.HasIndex(u => u.Abbreviation)
                .IsUnique();  // Ensure abbreviations are unique (if needed)

            // Seed data
            //builder.HasData(
            //    new Units { Id = Guid.NewGuid(), Name = "Kilogram", Abbreviation = "kg" },
            //    new Units { Id = Guid.NewGuid(), Name = "Gram", Abbreviation = "g" },
            //    new Units { Id = Guid.NewGuid(), Name = "Liter", Abbreviation = "l" },
            //    new Units { Id = Guid.NewGuid(), Name = "Milliliter", Abbreviation = "ml" },
            //    new Units { Id = Guid.NewGuid(), Name = "Piece", Abbreviation = "pcs" },
            //    new Units { Id = Guid.NewGuid(), Name = "Meter", Abbreviation = "m" },
            //    new Units { Id = Guid.NewGuid(), Name = "Centimeter", Abbreviation = "cm" },
            //    new Units { Id = Guid.NewGuid(), Name = "Millimeter", Abbreviation = "mm" },
            //    new Units { Id = Guid.NewGuid(), Name = "Kilometer", Abbreviation = "km" },
            //    new Units { Id = Guid.NewGuid(), Name = "Square Meter", Abbreviation = "m²" }
            //);

        }
    }
}
