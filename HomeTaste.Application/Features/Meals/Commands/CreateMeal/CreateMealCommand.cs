using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.CreateMeal
{
    public class CreateMealCommand : IRequest<Result<MealResponse>>
    {
        public MealRequest Request { get; set; }

        public CreateMealCommand(MealRequest request)
        {
            Request = request;
        }
    }
}
