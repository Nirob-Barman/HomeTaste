using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.BulkInsertUnits
{
    public class BulkInsertUnitsCommandHandler : IRequestHandler<BulkInsertUnitsCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BulkInsertUnitsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    var unitExists = await _unitOfWork.Repository<UnitEntity>().AnyAsync(u => u.Name == unit.Name || u.Abbreviation == unit.Abbreviation);

                    if (!unitExists)
                    {
                        newUnits.Add(unit);
                    }
                }
                if (!newUnits.Any())
                {
                    return Result<int>.Fail("All units already exist.", "No new units to insert", ResultType.Conflict);
                }

                await _unitOfWork.Repository<UnitEntity>().AddRangeAsync(newUnits);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newUnits.Count, "New units successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting units: {ex.Message}", "", ResultType.Failure);
            }
        }
    }
}
