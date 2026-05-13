using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Validators.Delivery
{
    public static class UpdateDeliveryStatusRequestValidator
    {
        public static List<string> Validate(UpdateDeliveryStatusRequest request)
        {
            var errors = new List<string>();

            if (!Enum.IsDefined(typeof(DeliveryStatus), request.Status))
                errors.Add("Invalid delivery status.");

            if (request.Notes?.Length > 500)
                errors.Add("Notes cannot exceed 500 characters.");

            return errors;
        }
    }
}
