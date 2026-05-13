using HomeTaste.Application.DTOs.MealManagement;

namespace HomeTaste.Application.DTOs.Test.MealAndMealCategory
{
    public class MealAndMealCategoryResponse
    {
        public MealCategoryResponse? MealCategory { get; set; }
        public MealResponse? Meal { get; set; }
    }
}
