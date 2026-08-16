using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.UpdateDeliveryStatus
{
    public class UpdateDeliveryStatusCommand : IRequest<Result<DeliveryAssignmentResponse>>
    {
        public Guid AssignmentId { get; set; }
        public UpdateDeliveryStatusRequest Request { get; set; }

        public UpdateDeliveryStatusCommand(Guid assignmentId, UpdateDeliveryStatusRequest request)
        {
            AssignmentId = assignmentId;
            Request = request;
        }
    }
}
