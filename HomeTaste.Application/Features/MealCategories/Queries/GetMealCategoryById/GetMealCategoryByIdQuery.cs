using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetMealCategoryById
{
    public class GetMealCategoryByIdQuery : IRequest<Result<MealCategoryResponse>>
    {
        public Guid Id { get; set; }

        public GetMealCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
