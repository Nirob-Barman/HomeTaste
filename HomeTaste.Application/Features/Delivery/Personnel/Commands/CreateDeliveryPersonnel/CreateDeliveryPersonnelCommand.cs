using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.CreateDeliveryPersonnel
{
    public class CreateDeliveryPersonnelCommand : IRequest<Result<DeliveryPersonnelResponse>>
    {
        public CreateDeliveryPersonnelRequest Request { get; set; }

        public CreateDeliveryPersonnelCommand(CreateDeliveryPersonnelRequest request)
        {
            Request = request;
        }
    }
}
