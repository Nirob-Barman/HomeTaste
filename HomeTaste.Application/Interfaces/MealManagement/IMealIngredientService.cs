using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IMealIngredientService
    {
        Task<Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>> GetAllMealIngredientsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<MealIngredientResponse>> GetMealIngredientByIdAsync(Guid id);
        Task<Result<MealIngredientResponse>> CreateMealIngredientAsync(MealIngredientRequest mealIngredientRequest);
        Task<Result<int>> BulkInsertPredefinedMealIngredientsAsync();
        Task<Result<MealIngredientResponse>> UpdateMealIngredientAsync(Guid id, MealIngredientRequest mealIngredientRequest);
        Task<Result<bool>> DeleteMealIngredientAsync(Guid id);
    }
}
