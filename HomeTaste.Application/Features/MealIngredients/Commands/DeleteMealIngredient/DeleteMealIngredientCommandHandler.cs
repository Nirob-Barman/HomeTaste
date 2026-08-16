using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.DeleteMealIngredient
{
    public class DeleteMealIngredientCommandHandler : IRequestHandler<DeleteMealIngredientCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMealIngredientCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteMealIngredientCommand command, CancellationToken cancellationToken)
        {
            var mealIngredient = await _context.MealIngredients.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (mealIngredient == null)
                throw new NotFoundException("Meal Ingredient not found");

            _context.MealIngredients.Remove(mealIngredient);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Meal Ingredient deleted successfully");
        }
    }
}
