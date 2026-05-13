using HomeTaste.Application.DTOs.UserProfile;

namespace HomeTaste.Application.Validators.UserProfile
{
    public static class UpdateProfileRequestValidator
    {
        public static List<string> Validate(UpdateProfileRequest request)
        {
            var errors = new List<string>();

            if (request.FirstName != null && request.FirstName.Trim().Length == 0)
                errors.Add("First name cannot be empty.");
            else if (request.FirstName?.Length > 100)
                errors.Add("First name cannot exceed 100 characters.");

            if (request.LastName != null && request.LastName.Trim().Length == 0)
                errors.Add("Last name cannot be empty.");
            else if (request.LastName?.Length > 100)
                errors.Add("Last name cannot exceed 100 characters.");

            if (request.DateOfBirth.HasValue && request.DateOfBirth.Value >= DateTime.UtcNow)
                errors.Add("Date of birth must be in the past.");

            if (request.DateOfBirth.HasValue && request.DateOfBirth.Value < new DateTime(1900, 1, 1))
                errors.Add("Date of birth is not valid.");

            if (request.PhoneNumber != null && request.PhoneNumber.Trim().Length > 20)
                errors.Add("Phone number cannot exceed 20 characters.");

            return errors;
        }
    }
}
