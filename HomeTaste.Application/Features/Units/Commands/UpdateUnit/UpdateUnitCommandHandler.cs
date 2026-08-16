using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result<UnitResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUnitCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UnitResponse>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var unitRequest = request.UnitRequest;

            // First, use Dapper to get the existing unit details
            var query = @"SELECT Id, Name, Abbreviation FROM Units WHERE Id = @Id";
            var parameters = new { Id = id };

            var unitResponse = await _unitOfWork.QueryAsync<UnitResponse>(query, parameters);

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            // Check if another unit with the same name or abbreviation exists using EF
            var existingUnit = await _unitOfWork.Repository<UnitEntity>().FirstOrDefaultAsync(u =>
                (u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation) && u.Id != id,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });

            if (existingUnit != null)
            {
                return Result<UnitResponse>.Fail("Unit with the same name or abbreviation already exists.", "Duplicate unit", ResultType.Conflict);
            }

            var checkDuplicateQuery = @"
        SELECT Id, Name, Abbreviation
        FROM Units
        WHERE (Name = @Name OR Abbreviation = @Abbreviation) AND Id != @Id";

            var duplicateUnit = await _unitOfWork.QueryFirstOrDefaultAsync<UnitResponse>(checkDuplicateQuery,
                new { unitRequest.Name, unitRequest.Abbreviation, Id = id });

            if (duplicateUnit != null)
            {
                return Result<UnitResponse>.Fail("Unit with the same name or abbreviation already exists.", "Duplicate unit", ResultType.Conflict);
            }

            // Begin the transaction with the UnitOfWork
            await _unitOfWork.BeginTransaction();

            try
            {
                // Now, use EF to update the unit
                var unitToUpdate = await _unitOfWork.Repository<UnitEntity>().GetByIdAsync(id);

                if (unitToUpdate == null)
                {
                    return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
                }

                // Update the entity using EF
                unitToUpdate.Name = unitRequest.Name ?? unitToUpdate.Name;
                unitToUpdate.Abbreviation = unitRequest.Abbreviation ?? unitToUpdate.Abbreviation;

                // Perform the update with EF
                _unitOfWork.Repository<UnitEntity>().Update(unitToUpdate);

                // Commit the transaction after the EF update
                await _unitOfWork.CommitAsync();

                // Return the updated unit details
                var updatedUnitResponse = new UnitResponse
                {
                    Id = unitToUpdate.Id,
                    Name = unitToUpdate.Name,
                    Abbreviation = unitToUpdate.Abbreviation
                };

                return Result<UnitResponse>.Ok(updatedUnitResponse, "Unit updated successfully", ResultType.Success);
            }
            catch (Exception ex)
            {
                // Rollback if something goes wrong
                await _unitOfWork.RollbackAsync();
                return Result<UnitResponse>.Fail($"An error occurred: {ex.Message}", "", ResultType.Failure);
            }
        }
    }
}
