namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealIngredient : BaseEntity
    {
        public Guid MealId { get; private set; }            // Meal associated with the ingredient (Foreign Key)
        public Guid IngredientId { get; private set; }     // Ingredient associated with the meal (Foreign Key)
        public decimal Quantity { get; private set; }        // Quantity of the ingredient used in the meal
        public Guid UnitId { get; private set; }             // Unit for the ingredient (e.g., grams, tablespoons, pieces)

        // Navigational properties
        public Meal? Meal { get; set; }
        public Ingredient? Ingredient { get; set; }
        public Units? Unit { get; set; }

        private MealIngredient() { } // EF Core

        public static MealIngredient Create(Guid mealId, Guid ingredientId, decimal quantity, Guid unitId)
        {
            return new MealIngredient
            {
                MealId = mealId,
                IngredientId = ingredientId,
                Quantity = quantity,
                UnitId = unitId
            };
        }

        public void UpdateDetails(Guid mealId, Guid ingredientId, decimal quantity, Guid unitId)
        {
            MealId = mealId;
            IngredientId = ingredientId;
            Quantity = quantity;
            UnitId = unitId;
        }
    }
}
