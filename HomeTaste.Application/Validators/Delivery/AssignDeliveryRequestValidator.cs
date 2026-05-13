using HomeTaste.Application.DTOs.Delivery;

namespace HomeTaste.Application.Validators.Delivery
{
    public static class AssignDeliveryRequestValidator
    {
        public static List<string> Validate(AssignDeliveryRequest request)
        {
            var errors = new List<string>();

            if (request.OrderId == Guid.Empty)
                errors.Add("OrderId is required.");

            if (request.DeliveryPersonnelId == Guid.Empty)
                errors.Add("DeliveryPersonnelId is required.");

            return errors;
        }
    }
}
