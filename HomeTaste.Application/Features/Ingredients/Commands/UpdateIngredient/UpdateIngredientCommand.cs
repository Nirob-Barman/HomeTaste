using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.UpdateIngredient
{
    public class UpdateIngredientCommand : IRequest<Result<IngredientResponse>>
    {
        public Guid Id { get; set; }
        public IngredientRequest IngredientRequest { get; set; }

        public UpdateIngredientCommand(Guid id, IngredientRequest ingredientRequest)
        {
            Id = id;
            IngredientRequest = ingredientRequest;
        }
    }
}
