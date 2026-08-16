using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.AssignDelivery
{
    public class AssignDeliveryCommand : IRequest<Result<DeliveryAssignmentResponse>>
    {
        public AssignDeliveryRequest Request { get; set; }

        public AssignDeliveryCommand(AssignDeliveryRequest request)
        {
            Request = request;
        }
    }
}
