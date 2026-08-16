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
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Unit deleted successfully", ResultType.Success);
        }
    }
}
