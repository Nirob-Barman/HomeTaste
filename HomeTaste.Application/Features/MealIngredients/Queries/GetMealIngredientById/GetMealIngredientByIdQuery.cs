using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Queries.GetMealIngredientById
{
    public record GetMealIngredientByIdQuery(Guid Id) : IRequest<Result<MealIngredientResponse>>;
}
