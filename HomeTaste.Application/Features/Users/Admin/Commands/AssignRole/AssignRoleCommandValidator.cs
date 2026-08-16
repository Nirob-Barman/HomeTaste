using FluentValidation;

namespace HomeTaste.Application.Features.Users.Admin.Commands.AssignRole
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty().WithMessage("UserId and RoleName are required");
            RuleFor(x => x.Request.RoleName).NotEmpty().WithMessage("UserId and RoleName are required");
        }
    }
}
