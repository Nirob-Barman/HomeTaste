using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopMeals
{
    public class GetTopMealsQuery : IRequest<Result<List<TopMealItem>>>
    {
        public int Top { get; set; } = 10;
    }
}
