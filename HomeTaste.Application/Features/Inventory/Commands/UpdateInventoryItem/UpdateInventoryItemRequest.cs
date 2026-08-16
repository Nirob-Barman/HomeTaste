namespace HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem
{
    public class UpdateInventoryItemRequest
    {
        public int StockCount { get; set; }
        public decimal? Price { get; set; }
    }
}
