namespace HomeTaste.Application.Settings
{
    public class JwtSettings
    {
        public AccessTokenSettings AccessToken { get; set; } = new AccessTokenSettings();
        public RefreshTokenSettings RefreshToken { get; set; } = new RefreshTokenSettings();
    }

    public class AccessTokenSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiryMinutes { get; set; }
        public string SigningAlgorithm { get; set; } = "HS256";
        public IDictionary<string, string> AdditionalClaims { get; set; } = new Dictionary<string, string>();
    }

    public class RefreshTokenSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
