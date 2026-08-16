namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealCategory : BaseEntity
    {
        public string? Name { get; private set; }          // Name of the Category (e.g., Vegan, Non-Veg, Gluten-Free)
        public string? Description { get; private set; }  // Optional description of the category
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }

        private MealCategory() { } // EF Core

        public static MealCategory Create(string? name, string? description)
        {
            return new MealCategory
            {
                Name = name,
                Description = description
            };
        }

        public void UpdateDetails(string? name, string? description)
        {
            Name = name ?? Name;
            Description = description ?? Description;
        }
    }
}
