using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class MealCategoryConfiguration : IEntityTypeConfiguration<MealCategory>
    {
        public void Configure(EntityTypeBuilder<MealCategory> builder)
        {
            builder.HasKey(mc => mc.Id);

            builder.Property(mc => mc.Name).IsRequired();

            builder.Property(mc => mc.Description).IsRequired(false);

            // Optionally, we can create a unique constraint on the Name to prevent duplicates
            builder.HasIndex(mc => mc.Name).IsUnique();  // Ensure category names are unique


            // Add seed data
            //builder.HasData(
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Vegan",
            //        Description = "Plant-based meals, no animal products."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Non-Veg",
            //        Description = "Meals that include meat or fish."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Gluten-Free",
            //        Description = "Meals without any gluten-containing ingredients."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Keto",
            //        Description = "Low-carb, high-fat meals for ketogenic diets."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Paleo",
            //        Description = "Meals based on eating whole foods that were available to our pre-agricultural ancestors."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Low-Carb",
            //        Description = "Meals with minimal carbohydrates, suitable for weight loss and blood sugar control."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Vegetarian",
            //        Description = "Meals that exclude meat but may include dairy and eggs."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Seafood",
            //        Description = "Meals featuring fish and other seafood."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Breakfast",
            //        Description = "Meals typically eaten in the morning, such as eggs, cereals, and pancakes."
            //    },
            //    new MealCategory
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Dessert",
            //        Description = "Sweet dishes typically eaten at the end of a meal."
            //    }
            //);

        }
    }
}
