using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.DeleteDeliveryPersonnel
{
    public class DeleteDeliveryPersonnelCommandHandler : IRequestHandler<DeleteDeliveryPersonnelCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDeliveryPersonnelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteDeliveryPersonnelCommand command, CancellationToken cancellationToken)
        {
            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            _context.DeliveryPersonnel.Remove(personnel);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Delivery personnel deleted successfully.");
        }
    }
}
