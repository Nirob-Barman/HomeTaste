using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetDeliveryZoneById
{
    public class GetDeliveryZoneByIdQueryHandler : IRequestHandler<GetDeliveryZoneByIdQuery, Result<DeliveryZoneResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetDeliveryZoneByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryZoneResponse>> Handle(GetDeliveryZoneByIdQuery request, CancellationToken cancellationToken)
        {
            var zone = await _context.DeliveryZones.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (zone == null)
                throw new NotFoundException("Zone not found.");

            return Result<DeliveryZoneResponse>.Ok(DeliveryZoneMapper.ToResponse(zone), "Zone retrieved.");
        }
    }
}
