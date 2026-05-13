

namespace HomeTaste.Domain.Entities.Support
{
    public class CategoryType
    {
        public Guid Id { get; set; } // Unique identifier for the category
        public string? Name { get; set; } // Name of the category (e.g., Food Quality, Delivery Issue)
        public string? Description { get; set; } // Optional description of the category type
    }
}
