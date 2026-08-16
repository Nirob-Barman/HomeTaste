using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.DeliveryZones.Commands.DeleteDeliveryZone
{
    public class DeleteDeliveryZoneCommandHandler : IRequestHandler<DeleteDeliveryZoneCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDeliveryZoneCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteDeliveryZoneCommand command, CancellationToken cancellationToken)
        {
            var zone = await _context.DeliveryZones.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (zone == null)
                throw new NotFoundException("Zone not found.");

            _context.DeliveryZones.Remove(zone);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Zone deleted.");
        }
    }
}
