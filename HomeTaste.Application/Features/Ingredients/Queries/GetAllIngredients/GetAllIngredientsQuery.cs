using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Ingredients.Queries.GetAllIngredients
{
    public record GetAllIngredientsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        string SortBy = "Id",
        string SortOrder = "ASC")
        : IRequest<Result<PaginatedResponse<IEnumerable<IngredientResponse>>>>;
}
