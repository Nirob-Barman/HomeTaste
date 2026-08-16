using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.DeleteDeliveryZone
{
    public record DeleteDeliveryZoneCommand(Guid Id) : IRequest<Result<bool>>;
}
