using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.DeleteMeal
{
    public record DeleteMealCommand(Guid Id) : IRequest<Result<bool>>;
}
