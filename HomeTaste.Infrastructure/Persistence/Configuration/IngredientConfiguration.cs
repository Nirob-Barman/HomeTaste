using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.HasIndex(i => i.Name)
                .IsUnique();  // Ensure ingredient names are unique

            // Add seed data
            //builder.HasData(
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Chicken",
            //        Description = "A popular source of protein, commonly used in various dishes.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/chicken.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Lettuce",
            //        Description = "A leafy green vegetable often used in salads and sandwiches.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/lettuce.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Peanuts",
            //        Description = "A legume known for being a common allergen.",
            //        IsAllergen = true,
            //        ImageUrl = "https://example.com/images/peanuts.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Tomato",
            //        Description = "A juicy fruit often used in salads, sauces, and sandwiches.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/tomato.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Garlic",
            //        Description = "A pungent herb used to flavor many dishes.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/garlic.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Olive Oil",
            //        Description = "A healthy fat often used in cooking and dressing.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/olive_oil.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Milk",
            //        Description = "A dairy product known for being a good source of calcium.",
            //        IsAllergen = true,
            //        ImageUrl = "https://example.com/images/milk.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Carrot",
            //        Description = "A crunchy orange vegetable, rich in beta-carotene.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/carrot.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Cheese",
            //        Description = "A dairy product made from milk, used in various dishes.",
            //        IsAllergen = true,
            //        ImageUrl = "https://example.com/images/cheese.jpg"
            //    },
            //    new Ingredient
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Spinach",
            //        Description = "A leafy green vegetable, known for its high iron content.",
            //        IsAllergen = false,
            //        ImageUrl = "https://example.com/images/spinach.jpg"
            //    }
            //);

        }
    }
}
