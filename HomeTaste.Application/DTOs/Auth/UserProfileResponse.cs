
namespace HomeTaste.Application.DTOs.Auth
{
    public class UserProfileResponse
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public List<string>? Roles { get; set; }
    }
}
