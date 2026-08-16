using FluentValidation;

namespace HomeTaste.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Request.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Request.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
