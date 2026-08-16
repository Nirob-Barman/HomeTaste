using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.MealCustomizations
{
    public class MealCustomizationOptionRequest
    {
        public Guid MealId { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public CustomizationOptionType OptionType { get; set; }
    }
}
