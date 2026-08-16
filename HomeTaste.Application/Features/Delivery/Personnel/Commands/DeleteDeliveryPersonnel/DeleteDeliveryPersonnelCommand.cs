using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.DeleteDeliveryPersonnel
{
    public record DeleteDeliveryPersonnelCommand(Guid Id) : IRequest<Result<bool>>;
}
