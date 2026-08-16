using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealCategories.Commands.BulkInsertMealCategories
{
    public class BulkInsertMealCategoriesCommandHandler : IRequestHandler<BulkInsertMealCategoriesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertMealCategoriesCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertMealCategoriesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Predefined Bengali Meal Categories
                var mealCategories = new List<(string Name, string Description, string ImageUrl)>
                {
                    ("Fish Curry", "Meals that focus on fish as the main ingredient, often cooked in flavorful spices, mustard oil, and sometimes coconut milk.", "https://example.com/images/fish_curry.jpg"),
                    ("Vegetarian", "Dishes made without meat or fish, often featuring vegetables, lentils, and spices.", "https://example.com/images/vegetarian.jpg"),
                    ("Bengali Sweets", "Sweet treats that are a staple in Bengali cuisine, including yogurt-based desserts and other sugary delights.", "https://example.com/images/bengali_sweets.jpg"),
                    ("Rice and Dal", "Simple, hearty meals consisting of rice and lentils, commonly paired with vegetables or chutneys.", "https://example.com/images/rice_and_dal.jpg"),
                    ("Snacks", "Small, typically fried or steamed items, often served as appetizers or street food.", "https://example.com/images/snacks.jpg"),
                    ("Biryani", "Fragrant rice dishes, typically made with meat (usually chicken or mutton), and rich in aromatic spices.", "https://example.com/images/biryani.jpg"),
                    ("Non-Vegetarian (Mutton/Chicken)", "Meals that include meats such as chicken or mutton, often prepared in rich curries or grilled styles.", "https://example.com/images/non_vegetarian.jpg"),
                    ("Traditional Bengali Delights", "Classic Bengali dishes that showcase the region's culinary heritage and flavors.", "https://example.com/images/traditional_bengali_delights.jpg"),
                    ("Comfort Food", "Simple, comforting dishes that are often eaten for daily meals, including rice and lentils.", "https://example.com/images/comfort_food.jpg"),
                };

                var newCategories = new List<MealCategory>();

                foreach (var (name, description, imageUrl) in mealCategories)
                {
                    var categoryExists = await _context.MealCategories.AnyAsync(c => c.Name == name, cancellationToken);

                    if (!categoryExists)
                    {
                        var category = MealCategory.Create(name, description);
                        category.ImageUrl = imageUrl;
                        newCategories.Add(category);
                    }
                }

                if (!newCategories.Any())
                {
                    return Result<int>.Fail("All meal categories already exist.", "No new categories to insert", ResultType.Conflict);
                }

                _context.MealCategories.AddRange(newCategories);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Ok(newCategories.Count, "New meal categories successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting meal categories: {ex.Message}", "", ResultType.Failure);
            }
        }
    }
}
