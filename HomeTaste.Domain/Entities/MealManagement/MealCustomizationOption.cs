using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealCustomizationOption : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public CustomizationOptionType OptionType { get; set; }

        public Meal? Meal { get; set; }
    }
}
