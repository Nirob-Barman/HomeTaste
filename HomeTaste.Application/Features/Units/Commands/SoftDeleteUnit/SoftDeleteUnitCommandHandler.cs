using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.SoftDeleteUnit
{
    public class SoftDeleteUnitCommandHandler : IRequestHandler<SoftDeleteUnitCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public SoftDeleteUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(SoftDeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _context.Units.FindAsync(new object?[] { request.Id }, cancellationToken);

            if (unit == null || unit.DeletedAt != null)
            {
                throw new NotFoundException("Unit not found");
            }

            unit.DeletedAt = DateTime.UtcNow;
            //unit.DeletedBy = Guid.NewGuid(); // Replace with logged-in user id

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Unit soft deleted successfully");
        }
    }
}
