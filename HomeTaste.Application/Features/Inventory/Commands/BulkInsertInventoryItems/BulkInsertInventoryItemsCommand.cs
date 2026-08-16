using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.BulkInsertInventoryItems
{
    public record BulkInsertInventoryItemsCommand : IRequest<Result<int>>;
}
