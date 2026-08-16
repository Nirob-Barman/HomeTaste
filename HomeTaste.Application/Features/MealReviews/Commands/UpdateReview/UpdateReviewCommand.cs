using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.UpdateReview
{
    public class UpdateReviewCommand : IRequest<Result<string>>
    {
        public Guid ReviewId { get; set; }
        public UpdateReviewRequest Request { get; set; }

        public UpdateReviewCommand(Guid reviewId, UpdateReviewRequest request)
        {
            ReviewId = reviewId;
            Request = request;
        }
    }
}
