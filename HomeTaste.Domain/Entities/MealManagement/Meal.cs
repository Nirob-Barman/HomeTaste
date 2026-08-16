namespace HomeTaste.Domain.Entities.MealManagement
{
    public class Meal : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public Guid CategoryId { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? PublicId { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public int? PreparationTime { get; private set; }
        public decimal? DiscountPrice { get; private set; }
        public int? Calories { get; private set; }

        // Navigation property to Category
        public MealCategory? MealCategory { get; set; }

        private Meal() { } // EF Core

        public static Meal Create(
            string? name,
            string? description,
            decimal price,
            Guid categoryId,
            string? imageUrl,
            bool isAvailable,
            int? preparationTime,
            decimal? discountPrice,
            int? calories)
        {
            return new Meal
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
                IsAvailable = isAvailable,
                PreparationTime = preparationTime,
                DiscountPrice = discountPrice,
                Calories = calories
            };
        }

        public void UpdateDetails(
            string? name,
            string? description,
            decimal price,
            Guid categoryId,
            string? imageUrl,
            bool isAvailable,
            int? preparationTime,
            decimal? discountPrice,
            int? calories)
        {
            Name = name ?? Name;
            Description = description ?? Description;
            Price = price;
            CategoryId = categoryId;
            ImageUrl = imageUrl ?? ImageUrl;
            IsAvailable = isAvailable;
            PreparationTime = preparationTime;
            DiscountPrice = discountPrice;
            Calories = calories;
        }
    }
}
