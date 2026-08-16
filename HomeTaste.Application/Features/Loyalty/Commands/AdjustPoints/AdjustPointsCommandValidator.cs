using FluentValidation;

namespace HomeTaste.Application.Features.Loyalty.Commands.AdjustPoints
{
    public class AdjustPointsCommandValidator : AbstractValidator<AdjustPointsCommand>
    {
        public AdjustPointsCommandValidator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Request.Points).NotEqual(0).WithMessage("Points must be a non-zero value (positive to add, negative to deduct).");

            RuleFor(x => x.Request.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
