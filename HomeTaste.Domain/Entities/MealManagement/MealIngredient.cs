using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealIngredient : BaseEntity
    {
        public Guid Id { get; set; }
        [ForeignKey("Meal")]
        public Guid MealId { get; set; }            // Meal associated with the ingredient (Foreign Key)
        [ForeignKey("Ingredient")]
        public Guid IngredientId { get; set; }     // Ingredient associated with the meal (Foreign Key)
        public decimal Quantity { get; set; }        // Quantity of the ingredient used in the meal
        [ForeignKey("Unit")]
        public Guid UnitId { get; set; }             // Unit for the ingredient (e.g., grams, tablespoons, pieces)


        // Navigational properties
        public Meal? Meal { get; set; }
        public Ingredient? Ingredient { get; set; }
        public Units? Unit { get; set; }
    }
}
