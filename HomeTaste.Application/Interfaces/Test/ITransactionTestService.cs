using HomeTaste.Application.DTOs.Test.MealAndMealCategory;
using HomeTaste.Application.DTOs.Test.UnitAndMealCategory;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Test
{
    public interface ITransactionTestService
    {
        Task<Result<UnitAndMealCategoryResponse>> CreateUnitAndMealCategoryAsync(UnitAndMealCategoryRequest request);
        Task<Result<MealAndMealCategoryResponse>> CreateMealAndMealCategoryAsync(MealAndMealCategoryRequest request);
    }
}
