using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.BulkInsertIngredients
{
    public class BulkInsertIngredientsCommand : IRequest<Result<int>>
    {
    }
}
