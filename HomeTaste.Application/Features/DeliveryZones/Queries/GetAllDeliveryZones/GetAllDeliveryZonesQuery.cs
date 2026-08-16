using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetAllDeliveryZones
{
    public class GetAllDeliveryZonesQuery : IRequest<Result<IEnumerable<DeliveryZoneResponse>>>
    {
    }
}
