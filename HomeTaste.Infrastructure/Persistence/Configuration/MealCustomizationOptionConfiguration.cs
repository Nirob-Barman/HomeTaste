using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class MealCustomizationOptionConfiguration : IEntityTypeConfiguration<MealCustomizationOption>
    {
        public void Configure(EntityTypeBuilder<MealCustomizationOption> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name).HasMaxLength(150);

            builder.Property(o => o.AdditionalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(o => o.Meal)
                .WithMany()
                .HasForeignKey(o => o.MealId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
