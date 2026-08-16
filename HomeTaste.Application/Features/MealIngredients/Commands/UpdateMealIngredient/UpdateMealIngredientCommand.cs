using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.UpdateMealIngredient
{
    public record UpdateMealIngredientCommand(Guid Id, Guid MealId, Guid IngredientId, decimal Quantity, Guid UnitId)
        : IRequest<Result<MealIngredientResponse>>;
}
