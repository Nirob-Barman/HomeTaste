using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.UpdateDeliveryStatus
{
    public record UpdateDeliveryStatusCommand(Guid AssignmentId, DeliveryStatus Status, string? Notes) : IRequest<Result<DeliveryAssignmentResponse>>;
}
