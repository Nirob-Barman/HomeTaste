namespace HomeTaste.Domain.Entities.MealManagement
{
    public class InventoryItem : BaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int StockCount { get; set; }
        public decimal Price { get; set; }

        public List<InventoryTransaction>? InventoryTransactions { get; set; } // History of all transactions (add/remove)
    }
}
