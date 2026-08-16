using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var unitResponse = await _context.Units
                .Where(u => u.Id == request.Id)
                .Select(u => new UnitResponse { Id = u.Id, Name = u.Name, Abbreviation = u.Abbreviation })
                .FirstOrDefaultAsync(cancellationToken);

            if (unitResponse == null)
            {
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            var unit = new Domain.Entities.Units { Id = unitResponse.Id };

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "Unit deleted successfully", ResultType.Success);
        }
    }
}
