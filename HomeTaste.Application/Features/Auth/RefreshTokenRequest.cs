namespace HomeTaste.Application.Features.Auth
{
    public record RefreshTokenRequest
    {
        public string? RefreshToken { get; set; }
    }
}
