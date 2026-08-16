using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetMyAssignedDeliveries
{
    public class GetMyAssignedDeliveriesQuery : IRequest<Result<IEnumerable<DeliveryAssignmentResponse>>>
    {
    }
}
