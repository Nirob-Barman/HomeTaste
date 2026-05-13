using HomeTaste.Application.DTOs.Address;

namespace HomeTaste.Application.Validators.Address
{
    public static class AddressRequestValidator
    {
        public static List<string> Validate(AddressRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.AddressLine1))
                errors.Add("Address line 1 is required.");
            else if (request.AddressLine1.Trim().Length > 200)
                errors.Add("Address line 1 cannot exceed 200 characters.");

            if (request.AddressLine2?.Length > 200)
                errors.Add("Address line 2 cannot exceed 200 characters.");

            if (string.IsNullOrWhiteSpace(request.City))
                errors.Add("City is required.");
            else if (request.City.Trim().Length > 100)
                errors.Add("City cannot exceed 100 characters.");

            if (request.State?.Length > 100)
                errors.Add("State cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(request.Country))
                errors.Add("Country is required.");
            else if (request.Country.Trim().Length > 100)
                errors.Add("Country cannot exceed 100 characters.");

            if (request.PostalCode?.Length > 20)
                errors.Add("Postal code cannot exceed 20 characters.");

            if (request.Latitude.HasValue && (request.Latitude.Value < -90 || request.Latitude.Value > 90))
                errors.Add("Latitude must be between -90 and 90.");

            if (request.Longitude.HasValue && (request.Longitude.Value < -180 || request.Longitude.Value > 180))
                errors.Add("Longitude must be between -180 and 180.");

            if (request.Label?.Length > 50)
                errors.Add("Label cannot exceed 50 characters.");

            return errors;
        }
    }
}
