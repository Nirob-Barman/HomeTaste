using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetAllDeliveryZones
{
    public record GetAllDeliveryZonesQuery : IRequest<Result<IEnumerable<DeliveryZoneResponse>>>;
}
