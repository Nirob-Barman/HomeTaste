using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Measurements;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities;

namespace HomeTaste.Application.Services.Measurements
{
    public class UnitService : IUnitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get all units
        public async Task<Result<PaginatedResponse<IEnumerable<UnitResponse>>>> GetAllUnitsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!)
        {
            var unitResponses = await _unitOfWork.Repository<Units>().GetAllAsync(unit => unit.DeletedAt == null, 
                unit => new UnitResponse
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    Abbreviation = unit.Abbreviation
                });

            //Pagination Based on Total Data:
            //var totalCount = unitResponses.Count();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                unitResponses = unitResponses.Where(unit =>
                    unit.Name!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    unit.Abbreviation!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            //Pagination Based on Search/ Filter Data
            var totalCount = unitResponses.Count();

            var pagedUnits = unitResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            var currentPageCount = pagedUnits.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<UnitResponse>>
            {
                Data = pagedUnits,
                MetaData = paginationMeta
            };

            if (!pagedUnits.Any())
            {
                return Result<PaginatedResponse<IEnumerable<UnitResponse>>>.Fail("No units found", "No units found", ResultType.NotFound);
            }

            return Result<PaginatedResponse<IEnumerable<UnitResponse>>>.Ok(response, "Units retrieved successfully", ResultType.Success);
        }



        // Get unit by Id
        public async Task<Result<UnitResponse>> GetUnitByIdAsync(Guid id)
        {
            var unitResponse = await _unitOfWork.Repository<Units>().GetByIdAsync(id, u => new UnitResponse
            {
                Id = u.Id,
                Name = u.Name,
                Abbreviation = u.Abbreviation
            });

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            return Result<UnitResponse>.Ok(unitResponse, "Unit retrieved successfully", ResultType.Success);
        }

        // Create a new unit
        public async Task<Result<UnitResponse>> CreateUnitAsync(UnitRequest unitRequest)
        {
            // Check if a unit with the same name or abbreviation already exists
            var existingUnit = await _unitOfWork.Repository<Units>().FirstOrDefaultAsync(u => u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });

            if (existingUnit != null)
            {
                return Result<UnitResponse>.Fail("Unit already exists with the same name or abbreviation.", "Duplicate unit", ResultType.Conflict);
            }

            var unit = new Units
            {
                Name = unitRequest.Name,
                Abbreviation = unitRequest.Abbreviation
            };

            await _unitOfWork.Repository<Units>().AddAsync(unit);
            await _unitOfWork.SaveChangesAsync();

            var unitResponse = new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            };

            return Result<UnitResponse>.Ok(unitResponse, "Unit created successfully", ResultType.Success);
        }


        public async Task<Result<int>> BulkInsertPredefinedUnitsAsync()
        {
            try
            {
                // Predefined units
                var units = new List<Units>
                {
                    new Units { Name = "Kilogram", Abbreviation = "kg" },
                    new Units { Name = "Gram", Abbreviation = "g" },
                    new Units { Name = "Liter", Abbreviation = "l" },
                    new Units { Name = "Milliliter", Abbreviation = "ml" },
                    new Units { Name = "Piece", Abbreviation = "pcs" },
                    new Units { Name = "Meter", Abbreviation = "m" },
                    new Units { Name = "Centimeter", Abbreviation = "cm" },
                    new Units { Name = "Millimeter", Abbreviation = "mm" },
                    new Units { Name = "Kilometer", Abbreviation = "km" },
                    new Units { Name = "Square Meter", Abbreviation = "m²" },
                    new Units { Name = "Pinch", Abbreviation = "pinch" },
                };

                var newUnits = new List<Units>();

                foreach (var unit in units)
                {
                    var unitExists = await _unitOfWork.Repository<Units>().AnyAsync(u => u.Name == unit.Name || u.Abbreviation == unit.Abbreviation);

                    if (!unitExists)
                    {
                        newUnits.Add(unit);
                    }
                }
                if (!newUnits.Any())
                {
                    return Result<int>.Fail("All units already exist.", "No new units to insert", ResultType.Conflict);
                }

                await _unitOfWork.Repository<Units>().AddRangeAsync(newUnits);
                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newUnits.Count, "New units successfully inserted", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting units: {ex.Message}", "", ResultType.Failure);
            }
        }


        // Update an existing unit
        public async Task<Result<UnitResponse>> UpdateUnitAsync(Guid id, UnitRequest unitRequest)
        {
            var unitResponse = await _unitOfWork.Repository<Units>().GetByIdAsync(id,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            var existingUnit = await _unitOfWork.Repository<Units>().FirstOrDefaultAsync(u => (u.Name == unitRequest.Name || u.Abbreviation == unitRequest.Abbreviation) && u.Id != id,
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

            var unit = new Units
            {
                Id = unitResponse!.Id,
                Name = unitRequest.Name ?? unitResponse.Name,
                Abbreviation = unitRequest.Abbreviation ?? unitResponse.Abbreviation
            };

            _unitOfWork.Repository<Units>().Update(unit);
            await _unitOfWork.SaveChangesAsync();

            var updatedUnitResponse =  new UnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Abbreviation = unit.Abbreviation
            };

            return Result<UnitResponse>.Ok(updatedUnitResponse, "Unit updated successfully", ResultType.Success);
        }

        public async Task<Result<UnitResponse>> UpdateUnitAsyncUsingEfGetAndDapperUpdate(Guid id, UnitRequest unitRequest)
        {
            var unitResponse = await _unitOfWork.Repository<Units>().GetByIdAsync(id,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            // Check if another unit with the same name or abbreviation exists
            var existingUnit = await _unitOfWork.Repository<Units>().FirstOrDefaultAsync(u =>
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

            // Begin the transaction with the UnitOfWork
            await _unitOfWork.BeginTransaction();

            try
            {
                // Use Dapper to update the unit in the database
                var updateQuery = @"UPDATE Units 
                            SET Name = @Name, Abbreviation = @Abbreviation 
                            WHERE Id = @Id";

                var parameters = new
                {
                    Name = unitRequest.Name ?? unitResponse.Name,
                    Abbreviation = unitRequest.Abbreviation ?? unitResponse.Abbreviation,
                    unitResponse.Id
                };

                // Execute the update query using Dapper
                await _unitOfWork.ExecuteAsync(updateQuery, parameters);

                // Commit the transaction after the Dapper update
                await _unitOfWork.CommitAsync();

                // Return the updated unit details
                var updatedUnitResponse = new UnitResponse
                {
                    Id = unitResponse.Id,
                    Name = unitRequest.Name ?? unitResponse.Name,
                    Abbreviation = unitRequest.Abbreviation ?? unitResponse.Abbreviation
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


        public async Task<Result<UnitResponse>> UpdateUnitAsyncUsingDapperGetAndEfUpdate(Guid id, UnitRequest unitRequest)
        {
            // First, use Dapper to get the existing unit details
            var query = @"SELECT Id, Name, Abbreviation FROM Units WHERE Id = @Id";
            var parameters = new { Id = id };

            var unitResponse = await _unitOfWork.QueryAsync<UnitResponse>(query, parameters);

            if (unitResponse == null)
            {
                return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            // Check if another unit with the same name or abbreviation exists using EF
            var existingUnit = await _unitOfWork.Repository<Units>().FirstOrDefaultAsync(u =>
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
                var unitToUpdate = await _unitOfWork.Repository<Units>().GetByIdAsync(id);

                if (unitToUpdate == null)
                {
                    return Result<UnitResponse>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
                }

                // Update the entity using EF
                unitToUpdate.Name = unitRequest.Name ?? unitToUpdate.Name;
                unitToUpdate.Abbreviation = unitRequest.Abbreviation ?? unitToUpdate.Abbreviation;

                // Perform the update with EF
                _unitOfWork.Repository<Units>().Update(unitToUpdate);

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

        public async Task<Result<bool>> SoftDeleteUnitAsync(Guid id)
        {
            var unit = await _unitOfWork.Repository<Units>().GetByIdAsync(id);

            if (unit == null || unit.DeletedAt != null)
            {
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            unit.DeletedAt = DateTime.UtcNow;
            //unit.DeletedBy = Guid.NewGuid(); // Replace with logged-in user id

            _unitOfWork.Repository<Units>().Update(unit);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Unit soft deleted successfully", ResultType.Success);
        }

        // Delete a unit
        public async Task<Result<bool>> HardDeleteUnitAsync(Guid id)
        {
            var unitResponse = await _unitOfWork.Repository<Units>().GetByIdAsync(id,
                u => new UnitResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Abbreviation = u.Abbreviation
                });
            if (unitResponse == null)
            {
                return Result<bool>.Fail("Unit not found", "Unit not found", ResultType.NotFound);
            }

            var unit = new Units
            {
                Id = unitResponse.Id
            };

            _unitOfWork.Repository<Units>().Remove(unit);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Unit deleted successfully", ResultType.Success);
        }

    }
}
