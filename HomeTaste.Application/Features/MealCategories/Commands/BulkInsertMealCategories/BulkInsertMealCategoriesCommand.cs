using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Commands.BulkInsertMealCategories
{
    public class BulkInsertMealCategoriesCommand : IRequest<Result<int>>
    {
    }
}
