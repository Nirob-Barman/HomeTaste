using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.DeleteInventoryItem
{
    public class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteInventoryItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteInventoryItemCommand command, CancellationToken cancellationToken)
        {
            var item = await _context.InventoryItems.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException("Item not found");
            }

            _context.InventoryItems.Remove(item);

            // Optionally, log the deletion as an "Inventory Deletion" transaction
            // Note: DeletedAt is set on this brand-new transaction record itself (not just the
            // item) - preserved as-is from the original service, which did the same thing.
            var transaction = InventoryTransaction.Create(item.Id, item.StockCount, item.Price, TransactionType.Deletion, "Item removed from inventory");
            transaction.DeletedAt = DateTime.UtcNow;
            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Item deleted successfully");
        }
    }
}
