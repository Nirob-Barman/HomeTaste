namespace HomeTaste.Application.Features.Users.Admin
{
    public record BanUserRequest
    {
        public string? Reason { get; set; }
    }
}
