

using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class Meal : BaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("MealCategory")]
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }


        // Navigation property to Category
        public MealCategory? MealCategory { get; set; }
    }
}
