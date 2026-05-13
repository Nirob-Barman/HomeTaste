
namespace HomeTaste.Application.DTOs.MealManagement.Inventory
{
    public class AddInventoryItemRequest
    {
        public string? Name { get; set; }
        public int StockCount { get; set; }
        public decimal Price { get; set; }
    }
}
