using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.BulkInsertMeals
{
    public class BulkInsertMealsCommand : IRequest<Result<int>>
    {
    }
}
