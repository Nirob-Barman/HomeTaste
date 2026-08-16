namespace HomeTaste.Application.Features.Users.Admin
{
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
}
