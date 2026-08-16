using FluentValidation;

namespace HomeTaste.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Request.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Request.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.Request.Role).NotEmpty().WithMessage("Role is required.");
        }
    }
}
