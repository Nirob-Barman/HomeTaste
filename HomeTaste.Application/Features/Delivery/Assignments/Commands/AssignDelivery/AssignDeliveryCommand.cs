using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.AssignDelivery
{
    public record AssignDeliveryCommand(Guid OrderId, Guid DeliveryPersonnelId) : IRequest<Result<DeliveryAssignmentResponse>>;
}
