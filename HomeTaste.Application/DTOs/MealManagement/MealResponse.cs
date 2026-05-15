namespace HomeTaste.Application.DTOs.MealManagement
{
    public class MealResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTime { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Calories { get; set; }
    }

    public class MealResponseWithMealCategory
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTime { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Calories { get; set; }
    }
}
