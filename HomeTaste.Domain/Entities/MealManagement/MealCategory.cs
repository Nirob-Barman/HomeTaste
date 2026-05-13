namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealCategory : BaseEntity
    {
        public Guid Id { get; set; }               // Unique Identifier for the Meal Category (Primary Key)
        public string? Name { get; set; }          // Name of the Category (e.g., Vegan, Non-Veg, Gluten-Free)
        public string? Description { get; set; }  // Optional description of the category
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
    }
}
