using FluentValidation;

namespace HomeTaste.Application.Features.UserProfile.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Request.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.Request.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Request.NewPassword)
                        .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
                        .Must(v => v.Any(char.IsUpper)).WithMessage("New password must contain at least one uppercase letter.")
                        .Must(v => v.Any(char.IsLower)).WithMessage("New password must contain at least one lowercase letter.")
                        .Must(v => v.Any(char.IsDigit)).WithMessage("New password must contain at least one digit.")
                        .Must(v => v.Any(c => !char.IsLetterOrDigit(c))).WithMessage("New password must contain at least one special character.");
                });

            RuleFor(x => x.Request)
                .Must(x => string.IsNullOrWhiteSpace(x.CurrentPassword) || string.IsNullOrWhiteSpace(x.NewPassword) || x.CurrentPassword != x.NewPassword)
                .WithMessage("New password must be different from the current password.")
                .OverridePropertyName("NewPassword");
        }
    }
}
