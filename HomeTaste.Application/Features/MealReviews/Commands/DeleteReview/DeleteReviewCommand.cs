using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.DeleteReview
{
    public record DeleteReviewCommand(Guid ReviewId) : IRequest<Result<string>>;
}
