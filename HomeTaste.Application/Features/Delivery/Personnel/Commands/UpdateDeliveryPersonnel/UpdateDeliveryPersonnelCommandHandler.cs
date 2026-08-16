using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateDeliveryPersonnel
{
    public class UpdateDeliveryPersonnelCommandHandler : IRequestHandler<UpdateDeliveryPersonnelCommand, Result<DeliveryPersonnelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDeliveryPersonnelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryPersonnelResponse>> Handle(UpdateDeliveryPersonnelCommand command, CancellationToken cancellationToken)
        {
            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            personnel.UpdateDetails(command.FullName, command.Phone, command.VehicleType, command.VehicleNumber);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<DeliveryPersonnelResponse>.Ok(DeliveryMapper.ToResponse(personnel), "Delivery personnel updated successfully.");
        }
    }
}
