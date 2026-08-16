using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public record UpdateIngredientCommand(
        Guid Id,
        string? Name,
        string? Description,
        bool IsAllergen,
        string? ImageUrl,
        string? PublicId) : IRequest<Result<IngredientResponse>>;
}
