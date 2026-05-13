using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Measurements
{
    public interface IUnitService
    {        
        Task<Result<PaginatedResponse<IEnumerable<UnitResponse>>>> GetAllUnitsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<UnitResponse>> GetUnitByIdAsync(Guid id);
        Task<Result<UnitResponse>> CreateUnitAsync(UnitRequest unitRequest);
        Task<Result<int>> BulkInsertPredefinedUnitsAsync();
        Task<Result<UnitResponse>> UpdateUnitAsync(Guid id, UnitRequest unitRequest);
        Task<Result<UnitResponse>> UpdateUnitAsyncUsingDapperGetAndEfUpdate(Guid id, UnitRequest unitRequest);
        Task<Result<bool>> SoftDeleteUnitAsync(Guid id);
        Task<Result<bool>> HardDeleteUnitAsync(Guid id);
    }
}
