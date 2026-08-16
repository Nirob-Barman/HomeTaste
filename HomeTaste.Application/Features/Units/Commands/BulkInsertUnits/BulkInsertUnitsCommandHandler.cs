using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnitEntity = HomeTaste.Domain.Entities.Units;

namespace HomeTaste.Application.Features.Units.Commands.BulkInsertUnits
{
    public class BulkInsertUnitsCommandHandler : IRequestHandler<BulkInsertUnitsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertUnitsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertUnitsCommand request, CancellationToken cancellationToken)
        {
            // Predefined units
            var units = new List<UnitEntity>
            {
                UnitEntity.Create("Kilogram", "kg"),
                UnitEntity.Create("Gram", "g"),
                UnitEntity.Create("Liter", "l"),
                UnitEntity.Create("Milliliter", "ml"),
                UnitEntity.Create("Piece", "pcs"),
                UnitEntity.Create("Meter", "m"),
                UnitEntity.Create("Centimeter", "cm"),
                UnitEntity.Create("Millimeter", "mm"),
                UnitEntity.Create("Kilometer", "km"),
                UnitEntity.Create("Square Meter", "m²"),
                UnitEntity.Create("Pinch", "pinch"),
            };

            var newUnits = new List<UnitEntity>();

            foreach (var unit in units)
            {
                var unitExists = await _context.Units.AnyAsync(u => u.Name == unit.Name || u.Abbreviation == unit.Abbreviation, cancellationToken);

                if (!unitExists)
                {
                    newUnits.Add(unit);
                }
            }
            if (!newUnits.Any())
            {
                throw new ConflictException("All units already exist.");
            }

            _context.Units.AddRange(newUnits);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(newUnits.Count, "New units successfully inserted");
        }
    }
}
