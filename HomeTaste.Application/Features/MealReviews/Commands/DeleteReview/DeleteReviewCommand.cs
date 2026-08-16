using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.DeleteReview
{
    public class DeleteReviewCommand : IRequest<Result<string>>
    {
        public Guid ReviewId { get; set; }

        public DeleteReviewCommand(Guid reviewId)
        {
            ReviewId = reviewId;
        }
    }
}
