using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetDeliveryZoneById
{
    public class GetDeliveryZoneByIdQuery : IRequest<Result<DeliveryZoneResponse>>
    {
        public Guid Id { get; set; }

        public GetDeliveryZoneByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
