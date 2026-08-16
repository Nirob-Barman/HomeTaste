using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.CreateIngredient
{
    public record CreateIngredientCommand(
        string? Name,
        string? Description,
        bool IsAllergen,
        string? ImageUrl,
        string? PublicId,
        FileUploadDto? File) : IRequest<Result<IngredientResponse>>;
}
