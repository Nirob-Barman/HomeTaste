using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Queries.GetAllMealIngredients
{
    public record GetAllMealIngredientsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>>;
}
