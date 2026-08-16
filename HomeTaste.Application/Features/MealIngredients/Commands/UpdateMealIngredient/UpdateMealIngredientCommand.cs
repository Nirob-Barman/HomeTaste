using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.UpdateMealIngredient
{
    public class UpdateMealIngredientCommand : IRequest<Result<MealIngredientResponse>>
    {
        public Guid Id { get; set; }
        public MealIngredientRequest Request { get; set; }

        public UpdateMealIngredientCommand(Guid id, MealIngredientRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
