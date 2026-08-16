using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.UpdateMeal
{
    public class UpdateMealCommand : IRequest<Result<MealResponse>>
    {
        public Guid Id { get; set; }
        public MealRequest Request { get; set; }

        public UpdateMealCommand(Guid id, MealRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
