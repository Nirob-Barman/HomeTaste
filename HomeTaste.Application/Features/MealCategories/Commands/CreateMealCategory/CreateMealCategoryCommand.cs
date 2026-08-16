using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.CreateMealCategory
{
    public record CreateMealCategoryCommand(string? Name, string? Description, string? ImageUrl)
        : IRequest<Result<MealCategoryResponse>>;
}
