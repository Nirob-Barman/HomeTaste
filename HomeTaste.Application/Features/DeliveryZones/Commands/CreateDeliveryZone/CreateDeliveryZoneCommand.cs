using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.CreateDeliveryZone
{
    public class CreateDeliveryZoneCommand : IRequest<Result<DeliveryZoneResponse>>
    {
        public CreateDeliveryZoneRequest Request { get; set; }

        public CreateDeliveryZoneCommand(CreateDeliveryZoneRequest request)
        {
            Request = request;
        }
    }
}
