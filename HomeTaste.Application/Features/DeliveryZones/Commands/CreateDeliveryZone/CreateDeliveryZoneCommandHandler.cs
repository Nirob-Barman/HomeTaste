using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using DeliveryZoneEntity = HomeTaste.Domain.Entities.Delivery.DeliveryZone;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.CreateDeliveryZone
{
    public class CreateDeliveryZoneCommandHandler : IRequestHandler<CreateDeliveryZoneCommand, Result<DeliveryZoneResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateDeliveryZoneCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryZoneResponse>> Handle(CreateDeliveryZoneCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var zone = DeliveryZoneEntity.Create(
                request.Name.Trim(),
                request.Description?.Trim(),
                request.IsActive,
                request.AllowedCities.Select(c => c.Trim().ToLowerInvariant()).ToList(),
                request.AllowedPostalCodes.Select(p => p.Trim().ToLowerInvariant()).ToList());

            _context.DeliveryZones.Add(zone);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<DeliveryZoneResponse>.Ok(DeliveryZoneMapper.ToResponse(zone), "Zone created.");
        }
    }
}
