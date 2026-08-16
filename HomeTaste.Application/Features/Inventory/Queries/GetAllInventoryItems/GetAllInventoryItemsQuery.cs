using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Queries.GetAllInventoryItems
{
    public class GetAllInventoryItemsQuery : IRequest<Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
    }
}
