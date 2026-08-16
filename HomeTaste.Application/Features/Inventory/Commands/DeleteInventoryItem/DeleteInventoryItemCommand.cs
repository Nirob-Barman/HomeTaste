using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.DeleteInventoryItem
{
    public class DeleteInventoryItemCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteInventoryItemCommand(Guid id)
        {
            Id = id;
        }
    }
}
