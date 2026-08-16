using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealIngredients.Commands.BulkInsertMealIngredients
{
    public class BulkInsertMealIngredientsCommandHandler : IRequestHandler<BulkInsertMealIngredientsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertMealIngredientsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertMealIngredientsCommand request, CancellationToken cancellationToken)
        {
            // Predefined MealIngredient data (with MealName, IngredientName, UnitName, Quantity)
            var predefined = new List<(string MealName, string IngredientName, decimal Quantity, string UnitName)>
            {
                ("Chicken Biryani", "Chicken", 350, "Gram"),
                ("Chicken Biryani", "Rice", 200, "Gram"),
                ("Chicken Biryani", "Onion", 100, "Gram"),
                ("Chicken Biryani", "Yogurt", 50, "Gram"),
                ("Chicken Biryani", "Ginger-Garlic Paste", 30, "Gram"),
                ("Chicken Biryani", "Biryani Masala", 15, "Gram"),
                ("Chicken Biryani", "Ghee", 30, "Gram"),
                ("Chicken Biryani", "Cinnamon", 2, "Piece"),
                ("Chicken Biryani", "Cloves", 4, "Piece"),
                ("Chicken Biryani", "Bay Leaf", 1, "Piece"),
                ("Chicken Biryani", "Cardamom", 2, "Piece"),
                //("Chicken Biryani", "Saffron", 1, "Pinch"),
                ("Chicken Biryani", "Coriander Leaves", 30, "Gram"),
                ("Chicken Biryani", "Mint Leaves", 30, "Gram"),
            };

            var newMealIngredients = new List<MealIngredient>();

            foreach (var (mealName, ingredientName, quantity, unitName) in predefined)
            {
                var meal = await _context.Meals.FirstOrDefaultAsync(m => m.Name == mealName, cancellationToken);
                var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Name == ingredientName, cancellationToken);
                var unit = await _context.Units.FirstOrDefaultAsync(u => u.Name == unitName, cancellationToken);

                // Check if all entities exist
                if (meal == null || ingredient == null || unit == null)
                {
                    throw new NotFoundException("Meal, Ingredient, or Unit not found.");
                }

                // Check if the MealIngredient already exists in the DB
                var existingMealIngredient = await _context.MealIngredients.FirstOrDefaultAsync(
                    mi => mi.MealId == meal.Id && mi.IngredientId == ingredient.Id && mi.UnitId == unit.Id, cancellationToken);

                if (existingMealIngredient == null)
                {
                    newMealIngredients.Add(MealIngredient.Create(meal.Id, ingredient.Id, quantity, unit.Id));
                }
            }

            // Check if there are new meal ingredients to insert
            if (!newMealIngredients.Any())
            {
                throw new ConflictException("No new meal ingredients to insert.");
            }

            _context.MealIngredients.AddRange(newMealIngredients);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(newMealIngredients.Count, "Meal ingredients successfully inserted");
        }
    }
}
