using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IMealCategoryService
    {
        Task<Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>> GetAllMealCategoriesAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!, string sortBy = "Id", string sortOrder = "ASC");

        Task<Result<MealCategoryResponse>> GetMealCategoryByIdAsync(Guid id);

        Task<Result<MealCategoryResponse>> CreateMealCategoryAsync(MealCategoryRequest mealCategoryRequest);
        Task<Result<int>> BulkInsertPredefinedMealCategoriesAsync();

        Task<Result<MealCategoryResponse>> UpdateMealCategoryAsync(Guid id, MealCategoryRequest mealCategoryRequest);

        Task<Result<bool>> DeleteMealCategoryAsync(Guid id);
    }
}
