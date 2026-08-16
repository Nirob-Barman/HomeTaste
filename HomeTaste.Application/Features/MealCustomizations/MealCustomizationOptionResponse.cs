using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.MealCustomizations
{
    public record MealCustomizationOptionResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public string? MealName { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }
        public bool IsAvailable { get; set; }
        public CustomizationOptionType OptionType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
