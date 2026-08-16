using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.DeleteDeliveryPersonnel
{
    public class DeleteDeliveryPersonnelCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteDeliveryPersonnelCommand(Guid id)
        {
            Id = id;
        }
    }
}
