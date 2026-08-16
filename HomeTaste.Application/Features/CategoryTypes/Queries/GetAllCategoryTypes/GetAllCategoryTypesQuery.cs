using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetAllCategoryTypes
{
    public record GetAllCategoryTypesQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>>;
}
