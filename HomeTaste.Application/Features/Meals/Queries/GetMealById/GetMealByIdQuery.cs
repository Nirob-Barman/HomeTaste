using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Queries.GetMealById
{
    public record GetMealByIdQuery(Guid Id) : IRequest<Result<MealResponse>>;
}
