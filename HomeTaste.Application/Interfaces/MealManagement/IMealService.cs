using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IMealService
    {
        Task<Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>> GetAllMealsAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null!);
        Task<Result<MealResponse>> GetMealByIdAsync(Guid id);
        Task<Result<MealResponse>> CreateMealAsync(MealRequest mealRequest, FileUploadDto? file);
        Task<Result<int>> BulkInsertPredefinedMealsAsync();
        Task<Result<MealResponse>> UpdateMealAsync(Guid id, MealRequest mealRequest);
        Task<Result<bool>> DeleteMealAsync(Guid id);
    }
}
