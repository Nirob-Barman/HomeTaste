using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.UpdateMealCategory
{
    public record UpdateMealCategoryCommand(Guid Id, string? Name, string? Description, string? ImageUrl)
        : IRequest<Result<MealCategoryResponse>>;
}
