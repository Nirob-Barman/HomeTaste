using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetDeliveryByOrderId
{
    public record GetDeliveryByOrderIdQuery(Guid OrderId) : IRequest<Result<DeliveryAssignmentResponse>>;
}
