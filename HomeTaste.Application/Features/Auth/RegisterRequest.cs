namespace HomeTaste.Application.Features.Auth
{
    public record RegisterRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; } = "Customer";
    }
}
