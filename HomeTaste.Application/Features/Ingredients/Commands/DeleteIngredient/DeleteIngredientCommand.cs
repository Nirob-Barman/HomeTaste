using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Commands.DeleteIngredient
{
    public class DeleteIngredientCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteIngredientCommand(Guid id)
        {
            Id = id;
        }
    }
}
