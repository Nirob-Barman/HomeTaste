using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetAllMealCategories
{
    public record GetAllMealCategoriesQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        string SortBy = "Id",
        string SortOrder = "ASC")
        : IRequest<Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>>;
}
