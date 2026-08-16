using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.GetAllDeliveryZones
{
    public class GetAllDeliveryZonesQueryHandler : IRequestHandler<GetAllDeliveryZonesQuery, Result<IEnumerable<DeliveryZoneResponse>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDeliveryZonesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<DeliveryZoneResponse>>> Handle(GetAllDeliveryZonesQuery request, CancellationToken cancellationToken)
        {
            var zones = await _context.DeliveryZones.ToListAsync(cancellationToken);
            return Result<IEnumerable<DeliveryZoneResponse>>.Ok(zones.Select(DeliveryZoneMapper.ToResponse), "Zones retrieved.");
        }
    }
}
