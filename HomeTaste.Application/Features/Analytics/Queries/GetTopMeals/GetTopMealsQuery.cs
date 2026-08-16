using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopMeals
{
    public record GetTopMealsQuery(int Top = 10) : IRequest<Result<List<TopMealItem>>>;
}
