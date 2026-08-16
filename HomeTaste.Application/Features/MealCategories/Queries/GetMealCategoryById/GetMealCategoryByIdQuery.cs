using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetMealCategoryById
{
    public record GetMealCategoryByIdQuery(Guid Id) : IRequest<Result<MealCategoryResponse>>;
}
