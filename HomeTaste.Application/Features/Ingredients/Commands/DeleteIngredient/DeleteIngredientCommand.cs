using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.DeleteIngredient
{
    public record DeleteIngredientCommand(Guid Id) : IRequest<Result<bool>>;
}
