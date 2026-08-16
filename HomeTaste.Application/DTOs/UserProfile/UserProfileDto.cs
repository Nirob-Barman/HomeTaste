namespace HomeTaste.Application.DTOs.UserProfile
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class UploadAvatarResponse
    {
        public string ProfileImageUrl { get; set; } = null!;
    }
}
