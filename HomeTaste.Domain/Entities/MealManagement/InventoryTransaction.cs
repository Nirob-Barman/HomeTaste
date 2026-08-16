
namespace HomeTaste.Domain.Entities.MealManagement
{
    public class InventoryTransaction : BaseEntity
    {
        public Guid InventoryItemId { get; set; }
        public InventoryItem? InventoryItem { get; set; }

        public int Quantity { get; set; } // Quantity of items added/removed
        public decimal UnitPrice { get; set; } // Price per unit during the transaction
        public decimal TotalPrice { get; set; } // Total price for the transaction (Quantity * UnitPrice)

        public int TransactionType { get; set; } // Type of transaction (e.g., "Restock", "Order Use", "Adjustment")
        public string? Notes { get; set; } // Additional notes for the transaction (optional)
    }
}
