using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.DTOs.Units;

namespace HomeTaste.Application.DTOs.Test.UnitAndMealCategory
{
    public class UnitAndMealCategoryRequest
    {
        public UnitRequest? Unit { get; set; }
        public MealCategoryRequest? MealCategory { get; set; }
    }
}
