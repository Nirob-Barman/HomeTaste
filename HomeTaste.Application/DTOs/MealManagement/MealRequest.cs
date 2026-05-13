
namespace HomeTaste.Application.DTOs.MealManagement
{
    public class MealRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class MealRequestWithCategoryName
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
