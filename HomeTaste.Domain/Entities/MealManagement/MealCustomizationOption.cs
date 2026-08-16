using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class MealCustomizationOption : BaseEntity
    {
        public Guid MealId { get; private set; }
        public string? Name { get; private set; }
        public decimal AdditionalPrice { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public CustomizationOptionType OptionType { get; private set; }

        public Meal? Meal { get; set; }

        private MealCustomizationOption() { } // EF Core

        public static MealCustomizationOption Create(Guid mealId, string? name, decimal additionalPrice, bool isAvailable, CustomizationOptionType optionType)
        {
            return new MealCustomizationOption
            {
                Id = Guid.NewGuid(),
                MealId = mealId,
                Name = name,
                AdditionalPrice = additionalPrice,
                IsAvailable = isAvailable,
                OptionType = optionType,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(Guid mealId, string? name, decimal additionalPrice, bool isAvailable, CustomizationOptionType optionType)
        {
            MealId = mealId;
            Name = name;
            AdditionalPrice = additionalPrice;
            IsAvailable = isAvailable;
            OptionType = optionType;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleAvailability()
        {
            IsAvailable = !IsAvailable;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
