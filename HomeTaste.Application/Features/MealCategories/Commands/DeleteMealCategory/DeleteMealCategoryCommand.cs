using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.DeleteMealCategory
{
    public record DeleteMealCategoryCommand(Guid Id) : IRequest<Result<bool>>;
}
