using HomeTaste.Application.DTOs.UserProfile;

namespace HomeTaste.Application.Validators.UserProfile
{
    public static class ChangePasswordRequestValidator
    {
        public static List<string> Validate(ChangePasswordRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                errors.Add("Current password is required.");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                errors.Add("New password is required.");
            }
            else
            {
                if (request.NewPassword.Length < 8)
                    errors.Add("New password must be at least 8 characters.");

                if (!request.NewPassword.Any(char.IsUpper))
                    errors.Add("New password must contain at least one uppercase letter.");

                if (!request.NewPassword.Any(char.IsLower))
                    errors.Add("New password must contain at least one lowercase letter.");

                if (!request.NewPassword.Any(char.IsDigit))
                    errors.Add("New password must contain at least one digit.");

                if (!request.NewPassword.Any(c => !char.IsLetterOrDigit(c)))
                    errors.Add("New password must contain at least one special character.");
            }

            if (!string.IsNullOrWhiteSpace(request.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(request.NewPassword) &&
                request.CurrentPassword == request.NewPassword)
                errors.Add("New password must be different from the current password.");

            return errors;
        }
    }
}
