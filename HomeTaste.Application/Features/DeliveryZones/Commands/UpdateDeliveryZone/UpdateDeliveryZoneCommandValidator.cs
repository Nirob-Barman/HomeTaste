using FluentValidation;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.UpdateDeliveryZone
{
    public class UpdateDeliveryZoneCommandValidator : AbstractValidator<UpdateDeliveryZoneCommand>
    {
        public UpdateDeliveryZoneCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
