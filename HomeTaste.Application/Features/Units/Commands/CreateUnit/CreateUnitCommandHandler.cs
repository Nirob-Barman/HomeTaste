using HomeTaste.Application.Common.Exceptions;
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
            var existingUnit = await _context.Units
                .Where(u => u.Name == request.Name || u.Abbreviation == request.Abbreviation)
                .Select(u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUnit != null)
            {
                throw new ConflictException("Unit already exists with the same name or abbreviation.");
            }

            var unit = UnitEntity.Create(request.Name, request.Abbreviation);

            _context.Units.Add(unit);
            await _context.SaveChangesAsync(cancellationToken);

            var unitResponse = new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            };

            return Result<UnitResponse>.Ok(unitResponse, "Unit created successfully");
        }
    }
}
