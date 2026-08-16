using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem
{
    public class AddInventoryItemCommand : IRequest<Result<InventoryItemResponse>>
    {
        public AddInventoryItemRequest Request { get; set; }

        public AddInventoryItemCommand(AddInventoryItemRequest request)
        {
            Request = request;
        }
    }
}
