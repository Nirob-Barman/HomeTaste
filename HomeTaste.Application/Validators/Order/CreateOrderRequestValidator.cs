using HomeTaste.Application.DTOs.Order;

namespace HomeTaste.Application.Validators.Order
{
    public static class CreateOrderRequestValidator
    {
        public static List<string> Validate(CreateOrderRequest request)
        {
            var errors = new List<string>();

            if (request.AddressId == Guid.Empty)
                errors.Add("A delivery address is required.");

            if (request.Items == null || request.Items.Count == 0)
            {
                errors.Add("At least one item is required.");
            }
            else
            {
                for (int i = 0; i < request.Items.Count; i++)
                {
                    var item = request.Items[i];

                    if (item.MealId == Guid.Empty)
                        errors.Add($"Item {i + 1}: MealId is required.");

                    if (item.Quantity <= 0)
                        errors.Add($"Item {i + 1}: Quantity must be at least 1.");

                    if (item.Quantity > 50)
                        errors.Add($"Item {i + 1}: Quantity cannot exceed 50.");

                    if (item.SpecialInstructions?.Length > 500)
                        errors.Add($"Item {i + 1}: Special instructions cannot exceed 500 characters.");
                }
            }

            if (request.PointsToRedeem < 0)
                errors.Add("Points to redeem cannot be negative.");

            if (request.Notes?.Length > 500)
                errors.Add("Notes cannot exceed 500 characters.");

            return errors;
        }
    }
}
