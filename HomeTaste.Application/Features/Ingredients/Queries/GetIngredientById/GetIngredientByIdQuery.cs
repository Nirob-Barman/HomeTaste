using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Queries.GetIngredientById
{
    public record GetIngredientByIdQuery(Guid Id) : IRequest<Result<IngredientResponse>>;
}
