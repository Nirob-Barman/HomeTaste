using FluentValidation;

namespace HomeTaste.Application.Features.Users.Admin.Commands.AssignRole
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId and RoleName are required");
            RuleFor(x => x.RoleName).NotEmpty().WithMessage("UserId and RoleName are required");
        }
    }
}
