using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.BulkInsertMealIngredients
{
    public record BulkInsertMealIngredientsCommand : IRequest<Result<int>>;
}
