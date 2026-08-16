using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Queries.GetIngredientById
{
    public class GetIngredientByIdQuery : IRequest<Result<IngredientResponse>>
    {
        public Guid Id { get; set; }

        public GetIngredientByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
