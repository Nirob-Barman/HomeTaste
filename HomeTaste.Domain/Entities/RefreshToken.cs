namespace HomeTaste.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string? Token { get; private set; }

        public string? UserId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime ExpiryDate { get; private set; }

        public bool IsRevoked { get; private set; }

        public DateTime? RevokedAt { get; set; }

        public string? RevokedBy { get; set; }  // The user/admin who revoked the token (if applicable)

        public bool IsActive { get; set; }

        private RefreshToken() { } // EF Core

        // Note: CreatedAt is intentionally left unset here — the original service never
        // populated it either, so every row defaults to DateTime.MinValue. Preserved as-is.
        public static RefreshToken Create(string token, string? userId, DateTime expiryDate)
        {
            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiryDate = expiryDate,
                IsRevoked = false
            };
        }

        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}
