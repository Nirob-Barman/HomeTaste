
namespace HomeTaste.Application.DTOs.MealManagement
{
    public class MealIngredientRequest
    {
        public Guid MealId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public Guid UnitId { get; set; }
    }

    public class MealIngredientBulkRequest
    {
        public string? MealName { get; set; }
        public string? IngredientName { get; set; }
        public decimal Quantity { get; set; }        
        public string? UnitName { get; set; }
    }
}
