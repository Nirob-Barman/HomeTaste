namespace HomeTaste.Application.Features.MealIngredients
{
    public class MealIngredientRequest
    {
        public Guid MealId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public Guid UnitId { get; set; }
    }
}
