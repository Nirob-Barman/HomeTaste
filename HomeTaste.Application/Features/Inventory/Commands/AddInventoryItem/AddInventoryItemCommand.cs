using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem
{
    public record AddInventoryItemCommand(string? Name, int StockCount, decimal Price)
        : IRequest<Result<InventoryItemResponse>>;
}
