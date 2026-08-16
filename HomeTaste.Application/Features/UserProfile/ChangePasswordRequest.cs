namespace HomeTaste.Application.Features.UserProfile
{
    public record ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
