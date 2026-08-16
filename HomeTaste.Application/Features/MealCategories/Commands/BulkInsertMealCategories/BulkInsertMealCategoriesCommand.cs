using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.BulkInsertMealCategories
{
    public record BulkInsertMealCategoriesCommand : IRequest<Result<int>>;
}
