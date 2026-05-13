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

    public class AdminUserResponse
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsLocked { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class BanUserRequest
    {
        public string? Reason { get; set; }
    }
}
