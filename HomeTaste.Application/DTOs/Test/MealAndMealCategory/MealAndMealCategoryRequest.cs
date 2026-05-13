using HomeTaste.Application.DTOs.MealCategories;
using HomeTaste.Application.DTOs.MealManagement;

namespace HomeTaste.Application.DTOs.Test.MealAndMealCategory
{
    public class MealAndMealCategoryRequest
    {
        public MealCategoryRequest? MealCategory { get; set; }
        public MealRequest? Meal { get; set; }
    }
}
