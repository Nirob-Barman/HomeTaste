using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.CreateMealIngredient
{
    public class CreateMealIngredientCommandHandler : IRequestHandler<CreateMealIngredientCommand, Result<MealIngredientResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateMealIngredientCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealIngredientResponse>> Handle(CreateMealIngredientCommand command, CancellationToken cancellationToken)
        {
            var meal = await _context.Meals.FindAsync(new object?[] { command.MealId }, cancellationToken);
            if (meal == null)
            {
                throw new NotFoundException("Meal not found");
            }

            var ingredient = await _context.Ingredients.FindAsync(new object?[] { command.IngredientId }, cancellationToken);
            if (ingredient == null)
            {
                throw new NotFoundException("Ingredient not found");
            }

            var unit = await _context.Units.FindAsync(new object?[] { command.UnitId }, cancellationToken);
            if (unit == null)
            {
                throw new NotFoundException("Unit not found");
            }

            var mealIngredient = MealIngredient.Create(command.MealId, command.IngredientId, command.Quantity, command.UnitId);

            _context.MealIngredients.Add(mealIngredient);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            };

            return Result<MealIngredientResponse>.Ok(response, "Meal Ingredient created successfully");
        }
    }
}
