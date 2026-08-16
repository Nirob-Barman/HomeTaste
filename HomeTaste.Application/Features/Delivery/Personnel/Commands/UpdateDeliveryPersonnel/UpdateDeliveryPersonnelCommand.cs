using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateDeliveryPersonnel
{
    public class UpdateDeliveryPersonnelCommand : IRequest<Result<DeliveryPersonnelResponse>>
    {
        public Guid Id { get; set; }
        public UpdateDeliveryPersonnelRequest Request { get; set; }

        public UpdateDeliveryPersonnelCommand(Guid id, UpdateDeliveryPersonnelRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
