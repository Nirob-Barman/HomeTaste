using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.ToggleAvailability
{
    public class ToggleAvailabilityCommandHandler : IRequestHandler<ToggleAvailabilityCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ToggleAvailabilityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(ToggleAvailabilityCommand command, CancellationToken cancellationToken)
        {
            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            personnel.ToggleAvailability();
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(personnel.IsAvailable, $"Marked as {(personnel.IsAvailable ? "available" : "unavailable")}.");
        }
    }
}
