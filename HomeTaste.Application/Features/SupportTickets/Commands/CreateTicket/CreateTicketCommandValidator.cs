using FluentValidation;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.SupportTickets.Commands.CreateTicket
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.Request.Subject)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Subject is required.")
                .Must(v => v!.Trim().Length <= 200).WithMessage("Subject cannot exceed 200 characters.");

            RuleFor(x => x.Request.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Description is required.")
                .Must(v => v!.Trim().Length <= 2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.Request.Priority)
                .IsInEnum().WithMessage("Invalid ticket priority.");

            RuleFor(x => x.Request.MobileNo)
                .Must(v => v == null || v.Trim().Length <= 20).WithMessage("Mobile number cannot exceed 20 characters.");
        }
    }
}
