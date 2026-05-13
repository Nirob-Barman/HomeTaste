namespace HomeTaste.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string? Token { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? RevokedBy { get; set; }  // The user/admin who revoked the token (if applicable)

        public bool IsActive { get; set; }
    }
}
