using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionsByMealId
{
    public record GetOptionsByMealIdQuery(Guid MealId) : IRequest<Result<IEnumerable<MealCustomizationOptionResponse>>>;
}
