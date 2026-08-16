using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Queries.GetAllMeals
{
    public record GetAllMealsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null, Guid? CategoryId = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>>;
}
