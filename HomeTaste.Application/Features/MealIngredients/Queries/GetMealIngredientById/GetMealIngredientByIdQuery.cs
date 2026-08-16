using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Queries.GetMealIngredientById
{
    public class GetMealIngredientByIdQuery : IRequest<Result<MealIngredientResponse>>
    {
        public Guid Id { get; set; }

        public GetMealIngredientByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
