namespace HomeTaste.Application.DTOs.MealManagement
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAllergen { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class IngredientPaginationResult
    {
        public int TotalCount { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAllergen { get; set; }
        public string? ImageUrl { get; set; }
    }

}
