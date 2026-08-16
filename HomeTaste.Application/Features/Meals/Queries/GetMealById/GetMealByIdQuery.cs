using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Queries.GetMealById
{
    public class GetMealByIdQuery : IRequest<Result<MealResponse>>
    {
        public Guid Id { get; set; }

        public GetMealByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
