using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UnitEntity = HomeTaste.Domain.Entities.Units;

namespace HomeTaste.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<UnitResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UnitResponse>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var unitRequest = request.UnitRequest;

            var existingUnit = await _context.Units
                .Where(u => u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation)
                .Select(u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUnit != null)
            {
                return Result<UnitResponse>.Fail("Unit already exists with the same name or abbreviation.", "Duplicate unit", ResultType.Conflict);
            }

            var unit = new UnitEntity
            {
                Name = unitRequest.Name,
                Abbreviation = unitRequest.Abbreviation
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync(cancellationToken);

            var unitResponse = new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            };

            return Result<UnitResponse>.Ok(unitResponse, "Unit created successfully", ResultType.Success);
        }
    }
}
