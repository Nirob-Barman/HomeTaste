using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IIngredientService
    {
        Task<Result<PaginatedResponse<IEnumerable<IngredientResponse>>>> GetAllIngredientsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!, string sortBy = "Id", string sortOrder = "ASC");
        Task<Result<IngredientResponse>> GetIngredientByIdAsync(Guid id);
        Task<Result<IngredientResponse>> CreateIngredientAsync(IngredientRequest ingredientRequest, FileUploadDto? file);
        Task<Result<int>> BulkInsertPredefinedIngredientsAsync();
        Task<Result<IngredientResponse>> UpdateIngredientAsync(Guid id, IngredientRequest ingredientRequest);
        Task<Result<bool>> DeleteIngredientAsync(Guid id);
    }
}
