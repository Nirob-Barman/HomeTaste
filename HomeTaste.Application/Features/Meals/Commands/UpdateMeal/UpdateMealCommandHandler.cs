using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Meals.Commands.UpdateMeal
{
    public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, Result<MealResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMealCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealResponse>> Handle(UpdateMealCommand command, CancellationToken cancellationToken)
        {
            var meal = await _context.Meals.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found");

            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { command.CategoryId }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("MealCategory not found");

            var existingMeal = await _context.Meals
                .AnyAsync(m => m.Name == command.Name && m.CategoryId == command.CategoryId && m.Id != command.Id, cancellationToken);
            if (existingMeal)
                throw new ConflictException("Meal with the same name already exists in this category.");

            meal.UpdateDetails(
                command.Name,
                command.Description,
                command.Price,
                command.CategoryId,
                command.ImageUrl,
                command.IsAvailable,
                command.PreparationTime,
                command.DiscountPrice,
                command.Calories);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealResponse(
                meal.Id,
                meal.Name,
                meal.Description,
                meal.Price,
                meal.ImageUrl,
                meal.CategoryId,
                meal.IsAvailable,
                meal.PreparationTime,
                meal.DiscountPrice,
                meal.Calories);

            return Result<MealResponse>.Ok(response, "Meal updated successfully");
        }
    }
}
