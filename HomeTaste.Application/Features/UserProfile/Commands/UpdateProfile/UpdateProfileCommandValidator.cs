using FluentValidation;

namespace HomeTaste.Application.Features.UserProfile.Commands.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .Must(v => v == null || v.Trim().Length > 0).WithMessage("First name cannot be empty.")
                .Must(v => v == null || v.Length <= 100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .Must(v => v == null || v.Trim().Length > 0).WithMessage("Last name cannot be empty.")
                .Must(v => v == null || v.Length <= 100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.")
                .GreaterThanOrEqualTo(new DateTime(1900, 1, 1)).WithMessage("Date of birth is not valid.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.PhoneNumber)
                .Must(v => v == null || v.Trim().Length <= 20).WithMessage("Phone number cannot exceed 20 characters.");
        }
    }
}
