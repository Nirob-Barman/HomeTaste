using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetDeliveryByOrderId
{
    public class GetDeliveryByOrderIdQueryHandler : IRequestHandler<GetDeliveryByOrderIdQuery, Result<DeliveryAssignmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetDeliveryByOrderIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryAssignmentResponse>> Handle(GetDeliveryByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var assignment = await _context.DeliveryAssignments
                .FirstOrDefaultAsync(a => a.OrderId == request.OrderId, cancellationToken);

            if (assignment == null)
                throw new NotFoundException("No delivery assignment found for this order.");

            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { assignment.DeliveryPersonnelId }, cancellationToken);
            return Result<DeliveryAssignmentResponse>.Ok(DeliveryMapper.ToResponse(assignment, personnel?.FullName), "Delivery assignment retrieved successfully.");
        }
    }
}
