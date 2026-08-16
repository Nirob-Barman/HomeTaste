using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetAllMealCategories
{
    public class GetAllMealCategoriesQuery : IRequest<Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "ASC";
    }
}
