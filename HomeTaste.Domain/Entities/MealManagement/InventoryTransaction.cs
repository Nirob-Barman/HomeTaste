using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.MealManagement
{
    public class InventoryTransaction : BaseEntity
    {
        public Guid InventoryItemId { get; private set; }
        public InventoryItem? InventoryItem { get; private set; }

        public int Quantity { get; private set; } // Quantity of items added/removed
        public decimal UnitPrice { get; private set; } // Price per unit during the transaction
        public decimal TotalPrice { get; private set; } // Total price for the transaction (Quantity * UnitPrice)

        public int TransactionType { get; private set; } // Type of transaction (e.g., "Restock", "Order Use", "Adjustment")
        public string? Notes { get; private set; } // Additional notes for the transaction (optional)

        private InventoryTransaction() { } // EF Core

        // Note: UnitPrice is intentionally never set here - the original service never populated
        // it either (only TotalPrice), so this preserves that as-is rather than "fixing" it.
        public static InventoryTransaction Create(Guid inventoryItemId, int quantity, decimal totalPrice, TransactionType transactionType, string? notes)
        {
            return new InventoryTransaction
            {
                InventoryItemId = inventoryItemId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                TransactionType = (int)transactionType,
                Notes = notes
            };
        }
    }
}
