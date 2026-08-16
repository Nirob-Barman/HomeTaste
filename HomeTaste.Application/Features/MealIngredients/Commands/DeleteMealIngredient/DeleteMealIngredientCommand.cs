using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.DeleteMealIngredient
{
    public class DeleteMealIngredientCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteMealIngredientCommand(Guid id)
        {
            Id = id;
        }
    }
}
