using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class MealIngredientConfiguration : IEntityTypeConfiguration<MealIngredient>
    {
        public void Configure(EntityTypeBuilder<MealIngredient> builder)
        {
            // Properties Configuration
            builder.Property(mi => mi.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,2)");  // Quantity of the ingredient, decimal type with 2 decimal places


            // Seed data for MealIngredient (Meal, Ingredient, Unit and Quantity)
            //builder.HasData(
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 1,        // Assuming Meal with Id = 1 exists (e.g., Vegan Burger)
            //        IngredientId = 1,  // Assuming Ingredient with Id = 1 exists (e.g., Vegan Patty)
            //        Quantity = 1.00m,  // 1 Vegan Patty for the Vegan Burger
            //        UnitId = 5         // Assuming Unit with Id = 5 exists (e.g., Piece)
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 1,        // Vegan Burger
            //        IngredientId = 2,  // Assuming Ingredient with Id = 2 exists (e.g., Lettuce)
            //        Quantity = 50.00m, // 50 grams of Lettuce
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 2,        // Chicken Wrap
            //        IngredientId = 3,  // Assuming Ingredient with Id = 3 exists (e.g., Chicken)
            //        Quantity = 200.00m, // 200 grams of Chicken
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 2,        // Chicken Wrap
            //        IngredientId = 4,  // Assuming Ingredient with Id = 4 exists (e.g., Tortilla)
            //        Quantity = 1.00m,  // 1 Tortilla
            //        UnitId = 5         // Unit: Piece
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 3,        // Gluten-Free Pizza
            //        IngredientId = 5,  // Assuming Ingredient with Id = 5 exists (e.g., Gluten-Free Dough)
            //        Quantity = 300.00m, // 300 grams of Gluten-Free Dough
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 3,        // Gluten-Free Pizza
            //        IngredientId = 6,  // Assuming Ingredient with Id = 6 exists (e.g., Tomato Sauce)
            //        Quantity = 150.00m, // 150 grams of Tomato Sauce
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 4,        // Keto Salad
            //        IngredientId = 7,  // Assuming Ingredient with Id = 7 exists (e.g., Avocado)
            //        Quantity = 100.00m, // 100 grams of Avocado
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 4,        // Keto Salad
            //        IngredientId = 8,  // Assuming Ingredient with Id = 8 exists (e.g., Chicken Breast)
            //        Quantity = 150.00m, // 150 grams of Chicken Breast
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 5,        // Paleo Chicken Wings
            //        IngredientId = 9,  // Assuming Ingredient with Id = 9 exists (e.g., Chicken Wings)
            //        Quantity = 250.00m, // 250 grams of Chicken Wings
            //        UnitId = 2         // Unit: Gram
            //    },
            //    new MealIngredient
            //    {
            //        Id = Guid.NewGuid(),
            //        MealId = 5,        // Paleo Chicken Wings
            //        IngredientId = 10, // Assuming Ingredient with Id = 10 exists (e.g., Paleo Sauce)
            //        Quantity = 50.00m, // 50 grams of Paleo Sauce
            //        UnitId = 2         // Unit: Gram
            //    }
            //);

        }
    }
}
