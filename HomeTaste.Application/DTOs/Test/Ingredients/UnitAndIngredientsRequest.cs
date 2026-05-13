
using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.DTOs.Units;

namespace HomeTaste.Application.DTOs.Test.Ingredients
{
    public class UnitAndIngredientsRequest
    {
        public UnitRequest? Unit { get; set; }
        public IngredientRequest? Ingredient { get; set; }
    }
}
