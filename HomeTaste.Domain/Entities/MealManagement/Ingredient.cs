namespace HomeTaste.Domain.Entities.MealManagement
{
    public class Ingredient : BaseEntity
    {
        public string? Name { get; private set; }           // Name of the Ingredient (e.g., Chicken, Lettuce)
        public string? Description { get; private set; }   // Description of the Ingredient
        public bool IsAllergen { get; private set; }
        public string? ImageUrl { get; private set; }      // Image URL (optional)
        public string? PublicId { get; private set; }

        private Ingredient() { } // EF Core

        public static Ingredient Create(string? name, string? description, bool isAllergen, string? imageUrl, string? publicId)
        {
            return new Ingredient
            {
                Name = name,
                Description = description,
                IsAllergen = isAllergen,
                ImageUrl = imageUrl,
                PublicId = publicId
            };
        }

        public void UpdateDetails(string? name, string? description, bool isAllergen, string? imageUrl)
        {
            Name = name ?? Name;
            Description = description ?? Description;
            IsAllergen = isAllergen;
            ImageUrl = imageUrl ?? ImageUrl;
        }
    }
}
