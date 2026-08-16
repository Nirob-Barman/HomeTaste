using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.SubmitReview
{
    public record SubmitReviewCommand(Guid MealId, Guid UserId, int Rating, string? Feedback)
        : IRequest<Result<string>>;
}
