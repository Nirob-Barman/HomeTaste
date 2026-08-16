using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Inventory.Commands.BulkInsertInventoryItems
{
    public class BulkInsertInventoryItemsCommandHandler : IRequestHandler<BulkInsertInventoryItemsCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public BulkInsertInventoryItemsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(BulkInsertInventoryItemsCommand command, CancellationToken cancellationToken)
        {
            // Predefined inventory items
            var predefined = new List<(string Name, int StockCount, decimal Price)>
            {
                ("Flour", 100, 1.99m), ("Sugar", 50, 2.49m), ("Salt", 200, 0.99m), ("Rice", 150, 1.59m),
                ("Olive Oil", 75, 6.99m), ("Butter", 120, 4.49m), ("Milk", 300, 1.29m), ("Eggs", 400, 2.99m),
                ("Honey", 60, 5.49m), ("Coconut Oil", 45, 7.99m), ("Baking Powder", 150, 1.79m), ("Vanilla Extract", 80, 4.99m),
                ("Chili Powder", 100, 2.19m), ("Paprika", 90, 3.39m), ("Black Pepper", 200, 1.89m), ("Garlic Powder", 180, 1.69m),
                ("Cinnamon", 130, 2.59m), ("Oats", 250, 2.79m), ("Peanut Butter", 110, 3.99m), ("Almonds", 95, 5.99m),
                ("Pasta", 300, 1.49m), ("Canned Tomatoes", 400, 0.79m), ("Chicken Breasts", 200, 8.49m), ("Ground Beef", 180, 5.79m),
                ("Lettuce", 220, 1.29m), ("Carrots", 250, 0.99m), ("Onions", 300, 1.09m), ("Tomatoes", 350, 1.39m),
                ("Avocados", 130, 2.59m), ("Cucumber", 150, 1.19m), ("Bell Peppers", 160, 2.29m), ("Spinach", 180, 2.99m),
                ("Broccoli", 200, 2.19m), ("Cheddar Cheese", 90, 3.69m), ("Mozzarella Cheese", 85, 4.29m), ("Parmesan Cheese", 70, 5.49m),
                ("Yogurt", 150, 2.49m), ("Sour Cream", 110, 1.79m), ("Cream Cheese", 90, 2.99m),
            };

            var existingNames = await _context.InventoryItems
                .Where(item => predefined.Select(p => p.Name).Contains(item.Name))
                .Select(item => item.Name)
                .ToListAsync(cancellationToken);

            var newInventoryItems = predefined
                .Where(p => !existingNames.Contains(p.Name))
                .Select(p => InventoryItem.Create(p.Name, p.StockCount, p.Price))
                .ToList();

            if (!newInventoryItems.Any())
            {
                throw new ConflictException("All inventory items already exist.");
            }

            _context.InventoryItems.AddRange(newInventoryItems);

            var transactions = newInventoryItems.Select(item =>
                InventoryTransaction.Create(item.Id, item.StockCount, item.Price, TransactionType.Restock, "Initial stock addition")
            ).ToList();

            _context.InventoryTransactions.AddRange(transactions);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(newInventoryItems.Count, "New inventory items successfully inserted and transactions added");
        }
    }
}
