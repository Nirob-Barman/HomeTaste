using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.SubmitReview
{
    public class SubmitReviewCommand : IRequest<Result<string>>
    {
        public SubmitReviewRequest Request { get; set; }

        public SubmitReviewCommand(SubmitReviewRequest request)
        {
            Request = request;
        }
    }
}
