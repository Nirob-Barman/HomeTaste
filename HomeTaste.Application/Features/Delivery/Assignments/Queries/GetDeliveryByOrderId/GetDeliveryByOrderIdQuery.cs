using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetDeliveryByOrderId
{
    public class GetDeliveryByOrderIdQuery : IRequest<Result<DeliveryAssignmentResponse>>
    {
        public Guid OrderId { get; set; }

        public GetDeliveryByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
