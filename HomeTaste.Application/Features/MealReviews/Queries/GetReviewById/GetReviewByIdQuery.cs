using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetReviewById
{
    public record GetReviewByIdQuery(Guid ReviewId) : IRequest<Result<DetailedReviewResponse>>;
}
