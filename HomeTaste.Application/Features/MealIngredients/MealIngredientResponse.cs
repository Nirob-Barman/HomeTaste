namespace HomeTaste.Application.Features.MealIngredients
{
    public record MealIngredientResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public string? MealName { get; set; }
        public Guid IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal Quantity { get; set; }
        public Guid UnitId { get; set; }
        public string? UnitName { get; set; }
    }
}
