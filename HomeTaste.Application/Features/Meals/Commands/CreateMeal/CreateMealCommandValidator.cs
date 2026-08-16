using FluentValidation;

namespace HomeTaste.Application.Features.Meals.Commands.CreateMeal
{
    public class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
    {
        public CreateMealCommandValidator()
        {
            RuleFor(x => x.Request.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Meal name is required.")
                .Must(v => v!.Trim().Length <= 200).WithMessage("Meal name cannot exceed 200 characters.");

            RuleFor(x => x.Request.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Request.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .LessThanOrEqualTo(100_000).WithMessage("Price seems unrealistically high.");

            RuleFor(x => x.Request.CategoryId)
                .NotEqual(Guid.Empty).WithMessage("Category is required.");
        }
    }
}
