using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.BulkInsertMealIngredients
{
    public class BulkInsertMealIngredientsCommand : IRequest<Result<int>>
    {
    }
}
