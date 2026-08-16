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
            try
            {
                // Predefined units
                var units = new List<UnitEntity>
                {
                    new UnitEntity { Name = "Kilogram", Abbreviation = "kg" },
                    new UnitEntity { Name = "Gram", Abbreviation = "g" },
                    new UnitEntity { Name = "Liter", Abbreviation = "l" },
                    new UnitEntity { Name = "Milliliter", Abbreviation = "ml" },
                    new UnitEntity { Name = "Piece", Abbreviation = "pcs" },
                    new UnitEntity { Name = "Meter", Abbreviation = "m" },
                    new UnitEntity { Name = "Centimeter", Abbreviation = "cm" },
                    new UnitEntity { Name = "Millimeter", Abbreviation = "mm" },
                    new UnitEntity { Name = "Kilometer", Abbreviation = "km" },
                    new UnitEntity { Name = "Square Meter", Abbreviation = "m²" },
                    new UnitEntity { Name = "Pinch", Abbreviation = "pinch" },
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
                    return Result<int>.Fail("All units already exist.", "No new units to insert", ResultType.Conflict);
                }

                _context.Units.AddRange(newUnits);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Ok(newUnits.Count, "New units successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting units: {ex.Message}", "", ResultType.Failure);
            }
        }
    }
}
