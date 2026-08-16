namespace HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem
{
    public record UpdateInventoryItemRequest
    {
        public int StockCount { get; set; }
        public decimal? Price { get; set; }
    }
}
