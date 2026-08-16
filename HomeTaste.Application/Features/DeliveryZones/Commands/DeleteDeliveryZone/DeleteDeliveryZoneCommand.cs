using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.DeleteDeliveryZone
{
    public class DeleteDeliveryZoneCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteDeliveryZoneCommand(Guid id)
        {
            Id = id;
        }
    }
}
