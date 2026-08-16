using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateLocation
{
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateLocationCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
        {
            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            personnel.UpdateLocation(command.Request.Latitude, command.Request.Longitude);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Location updated successfully.");
        }
    }
}
