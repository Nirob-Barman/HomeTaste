using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.UpdateDeliveryZone
{
    public class UpdateDeliveryZoneCommandHandler : IRequestHandler<UpdateDeliveryZoneCommand, Result<DeliveryZoneResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDeliveryZoneCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryZoneResponse>> Handle(UpdateDeliveryZoneCommand command, CancellationToken cancellationToken)
        {
            var zone = await _context.DeliveryZones.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (zone == null)
                throw new NotFoundException("Zone not found.");

            zone.UpdateDetails(
                command.Name.Trim(),
                command.Description?.Trim(),
                command.IsActive,
                command.AllowedCities.Select(c => c.Trim().ToLowerInvariant()).ToList(),
                command.AllowedPostalCodes.Select(p => p.Trim().ToLowerInvariant()).ToList());

            await _context.SaveChangesAsync(cancellationToken);

            return Result<DeliveryZoneResponse>.Ok(DeliveryZoneMapper.ToResponse(zone), "Zone updated.");
        }
    }
}
