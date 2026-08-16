using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem
{
    public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, Result<InventoryItemResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateInventoryItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<InventoryItemResponse>> Handle(UpdateInventoryItemCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var item = await _context.InventoryItems.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (item == null)
            {
                throw new NotFoundException("Item not found");
            }

            if (request.StockCount != item.StockCount)
            {
                int stockDifference = request.StockCount - item.StockCount;

                // If stock count is increased, create a "Restock" transaction
                var transactionType = stockDifference > 0 ? TransactionType.Restock : TransactionType.OrderUse;
                var transaction = InventoryTransaction.Create(
                    item.Id,
                    Math.Abs(stockDifference), // Store absolute difference
                    item.Price,
                    transactionType,
                    stockDifference > 0 ? "Stock restocked" : "Stock used for order");

                _context.InventoryTransactions.Add(transaction);
                item.UpdateStockCount(request.StockCount);
            }

            item.UpdatePrice(request.Price);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new InventoryItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                StockCount = item.StockCount,
                Price = item.Price
            };

            return Result<InventoryItemResponse>.Ok(response, "Item updated successfully");
        }
    }
}
