using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem
{
    public record UpdateInventoryItemCommand(Guid Id, int StockCount, decimal? Price)
        : IRequest<Result<InventoryItemResponse>>;
}
