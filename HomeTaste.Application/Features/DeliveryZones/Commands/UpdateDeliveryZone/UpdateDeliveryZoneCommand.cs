using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.UpdateDeliveryZone
{
    public record UpdateDeliveryZoneCommand(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive,
        List<string> AllowedCities,
        List<string> AllowedPostalCodes) : IRequest<Result<DeliveryZoneResponse>>;
}
