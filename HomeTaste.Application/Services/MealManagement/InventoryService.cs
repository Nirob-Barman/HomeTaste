using HomeTaste.Application.DTOs.MealManagement.Inventory;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.MealManagement;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Validators.MealManagement;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Application.Services.MealManagement
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<InventoryItem> _inventoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IRepository<InventoryItem> inventoryRepository, IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>> GetAllInventoryItemsAsync(int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null!)
        {
            var items = await _inventoryRepository.GetAllAsync(item => new InventoryItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                StockCount = item.StockCount,
                Price = item.Price
            });

            //var totalCount = items.Count();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                items = items.Where(item =>
                    item.Name!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = items.Count();

            var pagedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            paginationMeta.CurrentPageCount = pagedItems.Count();

            var response = new PaginatedResponse<IEnumerable<InventoryItemResponse>>
            {
                Data = pagedItems,
                MetaData = paginationMeta
            };

            if (!pagedItems.Any())
            {
                return Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>
                    .Fail("No inventory items found", "No inventory items found", ResultType.NotFound);
            }

            return Result<PaginatedResponse<IEnumerable<InventoryItemResponse>>>.Ok(response, "Inventory retrieved successfully", ResultType.Success);
        }

        public async Task<Result<InventoryItemResponse>> AddInventoryItemAsync(AddInventoryItemRequest request)
        {
            var errors = AddInventoryItemRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<InventoryItemResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var existingItem = await _inventoryRepository.FirstOrDefaultAsync(i => i.Name == request.Name);
            if (existingItem != null)
            {
                return Result<InventoryItemResponse>.Fail("Item with the same name already exists.", "Duplicate item", ResultType.Conflict);
            }

            var item = new InventoryItem
            {
                Name = request.Name,
                StockCount = request.StockCount,
                Price = request.Price
            };

            await _inventoryRepository.AddAsync(item);

            var transaction = new InventoryTransaction
            {
                InventoryItemId = item.Id,
                Quantity = request.StockCount,
                TotalPrice = item.Price,
                TransactionType = (int)TransactionType.Restock, // Transaction type is "Restock"
                Notes = "Initial stock addition",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);

            await _unitOfWork.SaveChangesAsync();

            var response = new InventoryItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                StockCount = item.StockCount,
                Price = item.Price
            };

            return Result<InventoryItemResponse>.Ok(response, "Item added to inventory successfully", ResultType.Success);
        }

        public async Task<Result<int>> BulkInsertInventoryItemsAsync()
        {
            try
            {
                // Predefined inventory items
                var inventoryItems = new List<InventoryItem>
                {
                    new InventoryItem { Name = "Flour", StockCount = 100, Price = 1.99m },
                    new InventoryItem { Name = "Sugar", StockCount = 50, Price = 2.49m },
                    new InventoryItem { Name = "Salt", StockCount = 200, Price = 0.99m },
                    new InventoryItem { Name = "Rice", StockCount = 150, Price = 1.59m },
                    new InventoryItem { Name = "Olive Oil", StockCount = 75, Price = 6.99m },
                    new InventoryItem { Name = "Butter", StockCount = 120, Price = 4.49m },
                    new InventoryItem { Name = "Milk", StockCount = 300, Price = 1.29m },
                    new InventoryItem { Name = "Eggs", StockCount = 400, Price = 2.99m },
                    new InventoryItem { Name = "Honey", StockCount = 60, Price = 5.49m },
                    new InventoryItem { Name = "Coconut Oil", StockCount = 45, Price = 7.99m },
                    new InventoryItem { Name = "Baking Powder", StockCount = 150, Price = 1.79m },
                    new InventoryItem { Name = "Vanilla Extract", StockCount = 80, Price = 4.99m },
                    new InventoryItem { Name = "Chili Powder", StockCount = 100, Price = 2.19m },
                    new InventoryItem { Name = "Paprika", StockCount = 90, Price = 3.39m },
                    new InventoryItem { Name = "Black Pepper", StockCount = 200, Price = 1.89m },
                    new InventoryItem { Name = "Garlic Powder", StockCount = 180, Price = 1.69m },
                    new InventoryItem { Name = "Cinnamon", StockCount = 130, Price = 2.59m },
                    new InventoryItem { Name = "Oats", StockCount = 250, Price = 2.79m },
                    new InventoryItem { Name = "Peanut Butter", StockCount = 110, Price = 3.99m },
                    new InventoryItem { Name = "Almonds", StockCount = 95, Price = 5.99m },
                    new InventoryItem { Name = "Pasta", StockCount = 300, Price = 1.49m },
                    new InventoryItem { Name = "Canned Tomatoes", StockCount = 400, Price = 0.79m },
                    new InventoryItem { Name = "Chicken Breasts", StockCount = 200, Price = 8.49m },
                    new InventoryItem { Name = "Ground Beef", StockCount = 180, Price = 5.79m },
                    new InventoryItem { Name = "Lettuce", StockCount = 220, Price = 1.29m },
                    new InventoryItem { Name = "Carrots", StockCount = 250, Price = 0.99m },
                    new InventoryItem { Name = "Onions", StockCount = 300, Price = 1.09m },
                    new InventoryItem { Name = "Tomatoes", StockCount = 350, Price = 1.39m },
                    new InventoryItem { Name = "Avocados", StockCount = 130, Price = 2.59m },
                    new InventoryItem { Name = "Cucumber", StockCount = 150, Price = 1.19m },
                    new InventoryItem { Name = "Bell Peppers", StockCount = 160, Price = 2.29m },
                    new InventoryItem { Name = "Spinach", StockCount = 180, Price = 2.99m },
                    new InventoryItem { Name = "Broccoli", StockCount = 200, Price = 2.19m },
                    new InventoryItem { Name = "Cheddar Cheese", StockCount = 90, Price = 3.69m },
                    new InventoryItem { Name = "Mozzarella Cheese", StockCount = 85, Price = 4.29m },
                    new InventoryItem { Name = "Parmesan Cheese", StockCount = 70, Price = 5.49m },
                    new InventoryItem { Name = "Yogurt", StockCount = 150, Price = 2.49m },
                    new InventoryItem { Name = "Sour Cream", StockCount = 110, Price = 1.79m },
                    new InventoryItem { Name = "Cream Cheese", StockCount = 90, Price = 2.99m }
                };

                var existingItems = await _unitOfWork.Repository<InventoryItem>()
                    .Where(item => inventoryItems.Select(i => i.Name).Contains(item.Name));

                var newInventoryItems = inventoryItems
                    .Where(item => !existingItems.Any(existing => existing.Name == item.Name));


                // If no new items, return conflict
                if (!newInventoryItems.Any())
                {
                    return Result<int>.Fail("All inventory items already exist.", "No new inventory items to insert", ResultType.Conflict);
                }

                // Bulk insert new items
                await _unitOfWork.Repository<InventoryItem>().AddRangeAsync(newInventoryItems);

                var transactions = newInventoryItems.Select(item => new InventoryTransaction
                {
                    InventoryItemId = item.Id,
                    Quantity = item.StockCount,
                    TotalPrice = item.Price,
                    TransactionType = (int)TransactionType.Restock,
                    Notes = "Initial stock addition",
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _unitOfWork.Repository<InventoryTransaction>().AddRangeAsync(transactions);

                await _unitOfWork.SaveChangesAsync();

                return Result<int>.Ok(newInventoryItems.Count(), "New inventory items successfully inserted and transactions added", ResultType.Success);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error occurred while bulk inserting inventory items: {ex.Message}", "", ResultType.Failure);
            }
        }

        public async Task<Result<InventoryItemResponse>> UpdateInventoryItemAsync(Guid id, UpdateInventoryItemRequest request)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
            {
                return Result<InventoryItemResponse>.Fail("Item not found", "Item not found", ResultType.NotFound);
            }

            if (request.StockCount != item!.StockCount)
            {
                int stockDifference = request.StockCount - item.StockCount;

                // If stock count is increased, create a "Restock" transaction
                int transactionType = stockDifference > 0 ? (int)TransactionType.Restock : (int)TransactionType.OrderUse;
                var transaction = new InventoryTransaction
                {
                    InventoryItemId = item.Id,
                    Quantity = Math.Abs(stockDifference), // Store absolute difference
                    TotalPrice = item.Price,
                    TransactionType = transactionType,
                    Notes = stockDifference > 0 ? "Stock restocked" : "Stock used for order",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
                item.StockCount = request.StockCount;
            }

            //item.StockCount = request.StockCount ?? item.StockCount;
            item.Price = request.Price ?? item.Price;

            _inventoryRepository.Update(item);
            await _unitOfWork.SaveChangesAsync();

            var response = new InventoryItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                StockCount = item.StockCount,
                Price = item.Price
            };

            return Result<InventoryItemResponse>.Ok(response, "Item updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteInventoryItemAsync(Guid id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
            {
                return Result<bool>.Fail("Item not found", "Item not found", ResultType.NotFound);
            }

            _inventoryRepository.Remove(item);

            // Optionally, log the deletion as an "Inventory Deletion" transaction
            var transaction = new InventoryTransaction
            {
                InventoryItemId = item.Id,
                Quantity = item.StockCount,
                TotalPrice = item.Price,
                TransactionType = (int)TransactionType.Deletion,
                Notes = "Item removed from inventory",
                CreatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Item deleted successfully", ResultType.Success);
        }
    }
}
