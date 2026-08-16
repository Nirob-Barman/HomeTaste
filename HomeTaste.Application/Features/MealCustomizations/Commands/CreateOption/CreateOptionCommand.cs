using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Commands.CreateOption
{
    public record CreateOptionCommand(Guid MealId, string? Name, decimal AdditionalPrice, bool IsAvailable, CustomizationOptionType OptionType)
        : IRequest<Result<MealCustomizationOptionResponse>>;
}
