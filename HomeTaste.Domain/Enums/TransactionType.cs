namespace HomeTaste.Domain.Enums
{
    public enum TransactionType
    {
        Restock = 1,    // Stock added back into inventory
        OrderUse = 2,   // Stock used for an order
        Adjustment = 3, // Adjustments made to stock (e.g., damaged goods, manual changes)
        Deletion = 4    // Item deletion from inventory (if relevant)
    }
}
