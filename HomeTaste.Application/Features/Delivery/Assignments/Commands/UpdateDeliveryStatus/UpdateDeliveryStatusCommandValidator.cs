using FluentValidation;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.UpdateDeliveryStatus
{
    public class UpdateDeliveryStatusCommandValidator : AbstractValidator<UpdateDeliveryStatusCommand>
    {
        public UpdateDeliveryStatusCommandValidator()
        {
            RuleFor(x => x.Request.Status).IsInEnum().WithMessage("Invalid delivery status.");
            RuleFor(x => x.Request.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
