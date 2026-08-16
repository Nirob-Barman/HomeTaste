using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem
{
    public class UpdateInventoryItemCommand : IRequest<Result<InventoryItemResponse>>
    {
        public Guid Id { get; set; }
        public UpdateInventoryItemRequest Request { get; set; }

        public UpdateInventoryItemCommand(Guid id, UpdateInventoryItemRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
