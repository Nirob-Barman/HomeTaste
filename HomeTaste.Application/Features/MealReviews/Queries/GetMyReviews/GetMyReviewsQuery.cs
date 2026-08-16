using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMyReviews
{
    public class GetMyReviewsQuery : IRequest<Result<IEnumerable<ReviewResponse>>>
    {
    }
}
