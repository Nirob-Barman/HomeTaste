using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Queries.GetAllInventoryItems
{
    public record GetAllInventoryItemsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>>;
}
