
namespace HomeTaste.Domain.Entities.MealManagement
{
    public class InventoryTransaction : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }

        public int Quantity { get; set; } // Quantity of items added/removed
        public decimal UnitPrice { get; set; } // Price per unit during the transaction
        public decimal TotalPrice { get; set; } // Total price for the transaction (Quantity * UnitPrice)

        public int TransactionType { get; set; } // Type of transaction (e.g., "Restock", "Order Use", "Adjustment")
        public string? Notes { get; set; } // Additional notes for the transaction (optional)
    }

    public enum TransactionType
    {
        Restock = 1,    // Stock added back into inventory
        OrderUse = 2,   // Stock used for an order
        Adjustment = 3, // Adjustments made to stock (e.g., damaged goods, manual changes)
        Deletion = 4    // Item deletion from inventory (if relevant)
    }
}
