
using HomeTaste.Application.DTOs.MealManagement.Inventory;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IInventoryService
    {
        Task<Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>> GetAllInventoryItemsAsync(int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null!);
        Task<Result<InventoryItemResponse>> AddInventoryItemAsync(AddInventoryItemRequest request);
        Task<Result<int>> BulkInsertInventoryItemsAsync();
        Task<Result<InventoryItemResponse>> UpdateInventoryItemAsync(Guid id, UpdateInventoryItemRequest request);
        Task<Result<bool>> DeleteInventoryItemAsync(Guid id);
    }
}
