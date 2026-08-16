using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.BulkInsertIngredients
{
    public record BulkInsertIngredientsCommand : IRequest<Result<int>>;
}
