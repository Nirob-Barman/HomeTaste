using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.DeleteInventoryItem
{
    public record DeleteInventoryItemCommand(Guid Id) : IRequest<Result<bool>>;
}
