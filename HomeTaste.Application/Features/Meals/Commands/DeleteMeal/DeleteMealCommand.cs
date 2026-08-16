using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.DeleteMeal
{
    public class DeleteMealCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteMealCommand(Guid id)
        {
            Id = id;
        }
    }
}
