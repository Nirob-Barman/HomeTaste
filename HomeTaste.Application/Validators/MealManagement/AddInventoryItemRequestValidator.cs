using HomeTaste.Application.DTOs.MealManagement.Inventory;

namespace HomeTaste.Application.Validators.MealManagement
{
    public static class AddInventoryItemRequestValidator
    {
        public static List<string> Validate(AddInventoryItemRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Item name is required.");
            else if (request.Name.Trim().Length > 200)
                errors.Add("Item name cannot exceed 200 characters.");

            if (request.StockCount < 0)
                errors.Add("Stock count cannot be negative.");

            if (request.Price < 0)
                errors.Add("Price cannot be negative.");

            return errors;
        }
    }
}
