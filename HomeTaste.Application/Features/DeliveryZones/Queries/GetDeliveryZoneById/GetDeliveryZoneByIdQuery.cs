using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetDeliveryZoneById
{
    public record GetDeliveryZoneByIdQuery(Guid Id) : IRequest<Result<DeliveryZoneResponse>>;
}
