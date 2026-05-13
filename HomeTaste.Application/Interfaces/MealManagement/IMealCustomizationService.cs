using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.MealManagement
{
    public interface IMealCustomizationService
    {
        Task<Result<IEnumerable<MealCustomizationOptionResponse>>> GetOptionsByMealIdAsync(Guid mealId);
        Task<Result<MealCustomizationOptionResponse>> GetOptionByIdAsync(Guid id);
        Task<Result<MealCustomizationOptionResponse>> CreateOptionAsync(MealCustomizationOptionRequest request);
        Task<Result<MealCustomizationOptionResponse>> UpdateOptionAsync(Guid id, MealCustomizationOptionRequest request);
        Task<Result<bool>> DeleteOptionAsync(Guid id);
        Task<Result<bool>> ToggleAvailabilityAsync(Guid id);
    }
}
