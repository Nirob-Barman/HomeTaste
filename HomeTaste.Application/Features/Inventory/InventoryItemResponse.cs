namespace HomeTaste.Application.Features.Inventory
{
    public class InventoryItemResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int StockCount { get; set; }
        public decimal Price { get; set; }
    }
}
