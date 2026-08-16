using FluentValidation;

namespace HomeTaste.Application.Features.Users.Admin.Commands.RemoveRole
{
    public class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
    {
        public RemoveRoleCommandValidator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId and RoleName are required");
            RuleFor(x => x.Request.RoleName).NotEmpty().WithMessage("UserId and RoleName are required");
        }
    }
}
