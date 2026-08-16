using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.CreateMealIngredient
{
    public record CreateMealIngredientCommand(Guid MealId, Guid IngredientId, decimal Quantity, Guid UnitId)
        : IRequest<Result<MealIngredientResponse>>;
}
