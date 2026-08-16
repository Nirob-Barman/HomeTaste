namespace HomeTaste.Application.Features.Ingredients
{
    public class IngredientResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAllergen { get; set; }
        public string? ImageUrl { get; set; }
    }
}
