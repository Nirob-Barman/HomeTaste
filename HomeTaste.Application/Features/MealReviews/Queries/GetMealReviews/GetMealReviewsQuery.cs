using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMealReviews
{
    public record GetMealReviewsQuery(Guid MealId) : IRequest<Result<IEnumerable<ReviewResponse>>>;
}
