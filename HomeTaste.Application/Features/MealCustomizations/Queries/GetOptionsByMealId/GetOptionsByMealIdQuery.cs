using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionsByMealId
{
    public class GetOptionsByMealIdQuery : IRequest<Result<IEnumerable<MealCustomizationOptionResponse>>>
    {
        public Guid MealId { get; set; }

        public GetOptionsByMealIdQuery(Guid mealId)
        {
            MealId = mealId;
        }
    }
}
