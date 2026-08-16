namespace HomeTaste.Application.Features.Ingredients
{
    public record IngredientRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAllergen { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicId { get; set; }
    }
}
