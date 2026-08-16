using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result<UnitResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UnitResponse>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var unitRequest = request.UnitRequest;

            // Get the existing unit details
            var unitResponse = await _context.Units
                .Where(u => u.Id == id)
                .Select(u => new UnitResponse { Id = u.Id, Name = u.Name, Abbreviation = u.Abbreviation })
                .FirstOrDefaultAsync(cancellationToken);

            if (unitResponse == null)
            {
                throw new NotFoundException("Unit not found");
            }

            // Check if another unit with the same name or abbreviation exists
            var existingUnit = await _context.Units
                .Where(u => (u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation) && u.Id != id)
                .Select(u => new UnitResponse { Id = u.Id, Name = u.Name, Abbreviation = u.Abbreviation })
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUnit != null)
            {
                throw new ConflictException("Unit with the same name or abbreviation already exists.");
            }

            var duplicateUnit = await _context.Units
                .Where(u => (u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation) && u.Id != id)
                .Select(u => new UnitResponse { Id = u.Id, Name = u.Name, Abbreviation = u.Abbreviation })
                .FirstOrDefaultAsync(cancellationToken);

            if (duplicateUnit != null)
            {
                throw new ConflictException("Unit with the same name or abbreviation already exists.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var unitToUpdate = await _context.Units.FindAsync(new object?[] { id }, cancellationToken);

                if (unitToUpdate == null)
                {
                    throw new NotFoundException("Unit not found");
                }

                unitToUpdate.UpdateDetails(unitRequest.Name, unitRequest.Abbreviation);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var updatedUnitResponse = new UnitResponse
                {
                    Id = unitToUpdate.Id,
                    Name = unitToUpdate.Name,
                    Abbreviation = unitToUpdate.Abbreviation
                };

                return Result<UnitResponse>.Ok(updatedUnitResponse, "Unit updated successfully");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
