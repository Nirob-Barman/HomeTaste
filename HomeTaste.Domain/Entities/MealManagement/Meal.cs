namespace HomeTaste.Domain.Entities.MealManagement
{
    public class Meal : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int? PreparationTime { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Calories { get; set; }

        // Navigation property to Category
        public MealCategory? MealCategory { get; set; }
    }
}
