using HomeTaste.Application.DTOs.Delivery;

namespace HomeTaste.Application.Validators.Delivery
{
    public static class CreateDeliveryPersonnelRequestValidator
    {
        public static List<string> Validate(CreateDeliveryPersonnelRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FullName))
                errors.Add("Full name is required.");
            else if (request.FullName.Trim().Length > 150)
                errors.Add("Full name cannot exceed 150 characters.");

            if (string.IsNullOrWhiteSpace(request.Phone))
                errors.Add("Phone number is required.");
            else if (request.Phone.Trim().Length > 20)
                errors.Add("Phone number cannot exceed 20 characters.");

            if (request.VehicleType?.Length > 50)
                errors.Add("Vehicle type cannot exceed 50 characters.");

            if (request.VehicleNumber?.Length > 50)
                errors.Add("Vehicle number cannot exceed 50 characters.");

            return errors;
        }
    }
}
