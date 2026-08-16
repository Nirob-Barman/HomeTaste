namespace HomeTaste.Domain.Entities.MealManagement
{
    public class InventoryItem : BaseEntity
    {
        public string? Name { get; private set; }
        public int StockCount { get; private set; }
        public decimal Price { get; private set; }

        public List<InventoryTransaction>? InventoryTransactions { get; set; } // History of all transactions (add/remove)

        private InventoryItem() { } // EF Core

        public static InventoryItem Create(string? name, int stockCount, decimal price)
        {
            return new InventoryItem
            {
                Name = name,
                StockCount = stockCount,
                Price = price
            };
        }

        public void UpdateStockCount(int stockCount)
        {
            StockCount = stockCount;
        }

        public void UpdatePrice(decimal? price)
        {
            Price = price ?? Price;
        }
    }
}
