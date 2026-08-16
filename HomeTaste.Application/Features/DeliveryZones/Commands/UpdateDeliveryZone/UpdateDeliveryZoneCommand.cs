using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.UpdateDeliveryZone
{
    public class UpdateDeliveryZoneCommand : IRequest<Result<DeliveryZoneResponse>>
    {
        public Guid Id { get; set; }
        public UpdateDeliveryZoneRequest Request { get; set; }

        public UpdateDeliveryZoneCommand(Guid id, UpdateDeliveryZoneRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
