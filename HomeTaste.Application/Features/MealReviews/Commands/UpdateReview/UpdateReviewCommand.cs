using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.UpdateReview
{
    public record UpdateReviewCommand(Guid ReviewId, int? Rating, string? Feedback)
        : IRequest<Result<string>>;
}
