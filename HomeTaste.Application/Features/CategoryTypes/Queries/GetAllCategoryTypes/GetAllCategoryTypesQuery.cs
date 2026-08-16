using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetAllCategoryTypes
{
    public class GetAllCategoryTypesQuery : IRequest<Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
    }
}
