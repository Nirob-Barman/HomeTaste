namespace HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem
{
    public record AddInventoryItemRequest
    {
        public string? Name { get; set; }
        public int StockCount { get; set; }
        public decimal Price { get; set; }
    }
}
