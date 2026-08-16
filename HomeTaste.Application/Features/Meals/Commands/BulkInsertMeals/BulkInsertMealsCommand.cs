using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.BulkInsertMeals
{
    public record BulkInsertMealsCommand : IRequest<Result<int>>;
}
