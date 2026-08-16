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
            var request = command.Request;

            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { request.CategoryId }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("Meal category not found.");

            var existingMeal = await _context.Meals
                .AnyAsync(m => m.Name == request.Name && m.CategoryId == request.CategoryId, cancellationToken);
            if (existingMeal)
                throw new ConflictException("Meal with the same name already exists in this category.");

            var meal = MealEntity.Create(
                request.Name,
                request.Description,
                request.Price,
                request.CategoryId,
                request.ImageUrl,
                request.IsAvailable,
                request.PreparationTime,
                request.DiscountPrice,
                request.Calories);

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
