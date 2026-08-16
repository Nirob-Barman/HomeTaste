using FluentValidation;

namespace HomeTaste.Application.Features.MealReviews.Commands.SubmitReview
{
    public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
    {
        public SubmitReviewCommandValidator()
        {
            RuleFor(x => x.Request.MealId).NotEqual(Guid.Empty).WithMessage("MealId is required.");

            RuleFor(x => x.Request.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Request.Feedback)
                .MaximumLength(1000).WithMessage("Feedback cannot exceed 1000 characters.");
        }
    }
}
