using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.BulkInsertInventoryItems
{
    public class BulkInsertInventoryItemsCommand : IRequest<Result<int>>
    {
    }
}
