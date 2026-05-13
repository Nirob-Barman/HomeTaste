namespace HomeTaste.Application.DTOs.MealManagement
{
    public class MealCategoryResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
