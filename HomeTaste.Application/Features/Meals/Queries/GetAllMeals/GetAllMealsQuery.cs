using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Queries.GetAllMeals
{
    public class GetAllMealsQuery : IRequest<Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
        public Guid? CategoryId { get; set; }
    }
}
