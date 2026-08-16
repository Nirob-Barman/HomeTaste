namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealIngredient : BaseEntity
    {
        public Guid MealId { get; set; }            // Meal associated with the ingredient (Foreign Key)
        public Guid IngredientId { get; set; }     // Ingredient associated with the meal (Foreign Key)
        public decimal Quantity { get; set; }        // Quantity of the ingredient used in the meal
        public Guid UnitId { get; set; }             // Unit for the ingredient (e.g., grams, tablespoons, pieces)


        // Navigational properties
        public Meal? Meal { get; set; }
        public Ingredient? Ingredient { get; set; }
        public Units? Unit { get; set; }
    }
}
