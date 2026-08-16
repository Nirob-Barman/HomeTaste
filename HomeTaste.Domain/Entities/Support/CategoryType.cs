namespace HomeTaste.Domain.Entities.Support
{
    public class CategoryType
    {
        public Guid Id { get; set; } // Unique identifier for the category
        public string? Name { get; private set; } // Name of the category (e.g., Food Quality, Delivery Issue)
        public string? Description { get; private set; } // Optional description of the category type

        private CategoryType() { } // EF Core

        public static CategoryType Create(string? name, string? description)
        {
            return new CategoryType
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
