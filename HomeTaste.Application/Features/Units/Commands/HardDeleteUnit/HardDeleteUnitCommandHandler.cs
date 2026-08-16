using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.HardDeleteUnit
{
    public class HardDeleteUnitCommandHandler : IRequestHandler<HardDeleteUnitCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public HardDeleteUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(HardDeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _context.Units.FindAsync(new object?[] { request.Id }, cancellationToken);

            if (unit == null)
            {
                throw new NotFoundException("Unit not found");
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Unit deleted successfully");
        }
    }
}
