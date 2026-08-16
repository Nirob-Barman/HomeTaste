using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetMyAssignedDeliveries
{
    public record GetMyAssignedDeliveriesQuery : IRequest<Result<IEnumerable<DeliveryAssignmentResponse>>>;
}
