using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.DeleteMealIngredient
{
    public record DeleteMealIngredientCommand(Guid Id) : IRequest<Result<bool>>;
}
