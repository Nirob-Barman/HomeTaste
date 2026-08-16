using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem
{
    public class AddInventoryItemCommandHandler : IRequestHandler<AddInventoryItemCommand, Result<InventoryItemResponse>>
    {
        private readonly IApplicationDbContext _context;

        public AddInventoryItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<InventoryItemResponse>> Handle(AddInventoryItemCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var existingItem = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);
            if (existingItem != null)
            {
                throw new ConflictException("Item with the same name already exists.");
            }

            var item = InventoryItem.Create(request.Name, request.StockCount, request.Price);
            _context.InventoryItems.Add(item);

            var transaction = InventoryTransaction.Create(item.Id, request.StockCount, item.Price, TransactionType.Restock, "Initial stock addition");
            _context.InventoryTransactions.Add(transaction);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new InventoryItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                StockCount = item.StockCount,
                Price = item.Price
            };

            return Result<InventoryItemResponse>.Ok(response, "Item added to inventory successfully");
        }
    }
}
