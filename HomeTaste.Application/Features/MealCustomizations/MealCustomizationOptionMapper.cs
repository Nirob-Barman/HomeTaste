using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Features.MealCustomizations
{
    internal static class MealCustomizationOptionMapper
    {
        public static MealCustomizationOptionResponse ToResponse(MealCustomizationOption option, string? mealName) => new()
        {
            Id = option.Id,
            MealId = option.MealId,
            MealName = mealName,
            Name = option.Name,
            AdditionalPrice = option.AdditionalPrice,
            IsAvailable = option.IsAvailable,
            OptionType = option.OptionType,
            CreatedAt = option.CreatedAt
        };
    }
}
