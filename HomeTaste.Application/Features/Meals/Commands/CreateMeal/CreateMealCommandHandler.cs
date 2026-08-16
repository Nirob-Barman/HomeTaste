using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MealEntity = HomeTaste.Domain.Entities.MealManagement.Meal;

namespace HomeTaste.Application.Features.Meals.Commands.CreateMeal
{
    public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, Result<MealResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateMealCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealResponse>> Handle(CreateMealCommand command, CancellationToken cancellationToken)
        {
            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { command.CategoryId }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("Meal category not found.");

            var existingMeal = await _context.Meals
                .AnyAsync(m => m.Name == command.Name && m.CategoryId == command.CategoryId, cancellationToken);
            if (existingMeal)
                throw new ConflictException("Meal with the same name already exists in this category.");

            var meal = MealEntity.Create(
                command.Name,
                command.Description,
                command.Price,
                command.CategoryId,
                command.ImageUrl,
                command.IsAvailable,
                command.PreparationTime,
                command.DiscountPrice,
                command.Calories);

            _context.Meals.Add(meal);
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

            return Result<MealResponse>.Ok(response, "Meal created successfully");
        }
    }
}
