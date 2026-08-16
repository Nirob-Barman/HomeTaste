namespace HomeTaste.Application.Features.Meals
{
    public record MealRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int? PreparationTime { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Calories { get; set; }
    }

    public record MealRequestWithCategoryName
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int? PreparationTime { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Calories { get; set; }
    }
}
