using FluentValidation;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.CreateDeliveryZone
{
    public class CreateDeliveryZoneCommandValidator : AbstractValidator<CreateDeliveryZoneCommand>
    {
        public CreateDeliveryZoneCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
