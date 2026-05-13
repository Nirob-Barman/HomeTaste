using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.MealManagement
{
    public class MealCustomizationOptionRequest
    {
        public Guid MealId { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public CustomizationOptionType OptionType { get; set; }
    }

    public class MealCustomizationOptionResponse
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
