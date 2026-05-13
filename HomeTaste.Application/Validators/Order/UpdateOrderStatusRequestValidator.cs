using HomeTaste.Application.DTOs.Order;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Validators.Order
{
    public static class UpdateOrderStatusRequestValidator
    {
        public static List<string> Validate(UpdateOrderStatusRequest request)
        {
            var errors = new List<string>();

            if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
                errors.Add("Invalid order status.");

            if (request.Status == OrderStatus.Cancelled && string.IsNullOrWhiteSpace(request.CancellationReason))
                errors.Add("Cancellation reason is required when cancelling an order.");

            if (request.CancellationReason?.Length > 500)
                errors.Add("Cancellation reason cannot exceed 500 characters.");

            return errors;
        }
    }
}
