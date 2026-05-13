using HomeTaste.Application.DTOs.Delivery;

namespace HomeTaste.Application.Validators.Delivery
{
    public static class UpdateLocationRequestValidator
    {
        public static List<string> Validate(UpdateLocationRequest request)
        {
            var errors = new List<string>();

            if (request.Latitude < -90 || request.Latitude > 90)
                errors.Add("Latitude must be between -90 and 90.");

            if (request.Longitude < -180 || request.Longitude > 180)
                errors.Add("Longitude must be between -180 and 180.");

            return errors;
        }
    }
}
