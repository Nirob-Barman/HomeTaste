using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Commands.CreateMealIngredient
{
    public class CreateMealIngredientCommand : IRequest<Result<MealIngredientResponse>>
    {
        public MealIngredientRequest Request { get; set; }

        public CreateMealIngredientCommand(MealIngredientRequest request)
        {
            Request = request;
        }
    }
}
