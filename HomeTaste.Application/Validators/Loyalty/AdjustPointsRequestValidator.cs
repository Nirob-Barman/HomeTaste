using HomeTaste.Application.DTOs.Loyalty;

namespace HomeTaste.Application.Validators.Loyalty
{
    public static class AdjustPointsRequestValidator
    {
        public static List<string> Validate(AdjustPointsRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.UserId))
                errors.Add("UserId is required.");

            if (request.Points == 0)
                errors.Add("Points must be a non-zero value (positive to add, negative to deduct).");

            if (request.Description?.Length > 500)
                errors.Add("Description cannot exceed 500 characters.");

            return errors;
        }
    }
}
