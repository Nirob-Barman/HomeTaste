using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class MealConfiguration : IEntityTypeConfiguration<Meal>
    {
        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasKey(m => m.Id);

            //builder.Property(m => m.CategoryId)
            //    .IsRequired();

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            // Add any additional configurations or indexes as necessary

            //builder.HasData(
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Vegan Burger",
            //        Description = "A plant-based burger made with a delicious vegan patty.",
            //        Price = 12.99m,
            //        CategoryId = 1, // Assumes the 'Vegan' category exists in the database
            //        ImageUrl = "https://example.com/images/vegan_burger.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Chicken Wrap",
            //        Description = "Grilled chicken wrapped in a fresh tortilla with veggies.",
            //        Price = 9.99m,
            //        CategoryId = 2, // Assumes the 'Non-Veg' category exists
            //        ImageUrl = "https://example.com/images/chicken_wrap.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Gluten-Free Pizza",
            //        Description = "A gluten-free pizza with fresh veggies and cheese.",
            //        Price = 14.99m,
            //        CategoryId = 3, // Assumes the 'Gluten-Free' category exists
            //        ImageUrl = "https://example.com/images/gluten_free_pizza.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Keto Salad",
            //        Description = "A low-carb salad with avocados, chicken, and keto-friendly dressing.",
            //        Price = 10.99m,
            //        CategoryId = 4, // Assumes the 'Keto' category exists
            //        ImageUrl = "https://example.com/images/keto_salad.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Paleo Chicken Wings",
            //        Description = "Delicious chicken wings made with paleo-friendly ingredients.",
            //        Price = 11.49m,
            //        CategoryId = 5, // Assumes the 'Paleo' category exists
            //        ImageUrl = "https://example.com/images/paleo_chicken_wings.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Low-Carb Burger",
            //        Description = "A burger made with a low-carb bun and fresh ingredients.",
            //        Price = 13.49m,
            //        CategoryId = 6, // Assumes the 'Low-Carb' category exists
            //        ImageUrl = "https://example.com/images/low_carb_burger.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Vegetarian Pasta",
            //        Description = "A pasta dish with fresh veggies and a light olive oil dressing.",
            //        Price = 11.99m,
            //        CategoryId = 7, // Assumes the 'Vegetarian' category exists
            //        ImageUrl = "https://example.com/images/vegetarian_pasta.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Seafood Paella",
            //        Description = "A traditional Spanish paella with seafood and saffron rice.",
            //        Price = 19.99m,
            //        CategoryId = 8, // Assumes the 'Seafood' category exists
            //        ImageUrl = "https://example.com/images/seafood_paella.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Breakfast Burrito",
            //        Description = "A hearty breakfast burrito filled with eggs, sausage, and cheese.",
            //        Price = 8.99m,
            //        CategoryId = 9, // Assumes the 'Breakfast' category exists
            //        ImageUrl = "https://example.com/images/breakfast_burrito.jpg"
            //    },
            //    new Meal
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Chocolate Cake",
            //        Description = "A rich and moist chocolate cake, perfect for dessert.",
            //        Price = 6.99m,
            //        CategoryId = 10, // Assumes the 'Dessert' category exists
            //        ImageUrl = "https://example.com/images/chocolate_cake.jpg"
            //    }
            //);

        }
    }
}
