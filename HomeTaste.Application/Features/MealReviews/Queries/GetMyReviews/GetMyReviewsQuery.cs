using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMyReviews
{
    public record GetMyReviewsQuery : IRequest<Result<IEnumerable<ReviewResponse>>>;
}
