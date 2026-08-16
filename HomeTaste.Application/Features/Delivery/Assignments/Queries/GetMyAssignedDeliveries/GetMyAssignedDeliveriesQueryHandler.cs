using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Delivery.Assignments.Queries.GetMyAssignedDeliveries
{
    public class GetMyAssignedDeliveriesQueryHandler : IRequestHandler<GetMyAssignedDeliveriesQuery, Result<IEnumerable<DeliveryAssignmentResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyAssignedDeliveriesQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<DeliveryAssignmentResponse>>> Handle(GetMyAssignedDeliveriesQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var personnel = await _context.DeliveryPersonnel.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("No delivery personnel profile found for this user.");

            var assignments = await _context.DeliveryAssignments
                .Where(a => a.DeliveryPersonnelId == personnel.Id)
                .ToListAsync(cancellationToken);

            var response = assignments.Select(a => DeliveryMapper.ToResponse(a, personnel.FullName));
            return Result<IEnumerable<DeliveryAssignmentResponse>>.Ok(response, "Assignments retrieved successfully.");
        }
    }
}
