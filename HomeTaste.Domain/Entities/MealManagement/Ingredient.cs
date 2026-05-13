

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class Ingredient : BaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }           // Name of the Ingredient (e.g., Chicken, Lettuce)
        public string? Description { get; set; }   // Description of the Ingredient
        public bool IsAllergen { get; set; }
        public string? ImageUrl { get; set; }      // Image URL (optional)
        public string? PublicId { get; set; }
    }
}
