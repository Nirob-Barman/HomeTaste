namespace HomeTaste.Application.Features.MealCategories
{
    public record MealCategoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
