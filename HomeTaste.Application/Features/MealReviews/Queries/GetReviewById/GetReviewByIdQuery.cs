using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetReviewById
{
    public class GetReviewByIdQuery : IRequest<Result<DetailedReviewResponse>>
    {
        public Guid ReviewId { get; set; }

        public GetReviewByIdQuery(Guid reviewId)
        {
            ReviewId = reviewId;
        }
    }
}
