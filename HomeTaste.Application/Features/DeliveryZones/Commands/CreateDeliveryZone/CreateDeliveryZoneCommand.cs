using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.CreateDeliveryZone
{
    public record CreateDeliveryZoneCommand(
        string Name,
        string? Description,
        bool IsActive,
        List<string> AllowedCities,
        List<string> AllowedPostalCodes) : IRequest<Result<DeliveryZoneResponse>>;
}
