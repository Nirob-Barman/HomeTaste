namespace HomeTaste.Domain.Entities
{
    public class LoginAudit
    {
        public Guid Id { get; set; }

        public string? UserId { get; private set; }

        public DateTime LoginTime { get; private set; }

        public string? IPAddress { get; private set; }

        public string? DeviceInfo { get; set; }

        public string? UserAgent { get; private set; }

        public bool IsSuccessful { get; private set; }

        public string? Location { get; set; }  // Optional: Geolocation of the user (could use IP-based geolocation)

        public string? ErrorMessage { get; set; }  // Optional: If login failed, this can store the error message (e.g., "Invalid credentials")

        public string? DeviceFingerprint { get; set; }  // Optional: A unique identifier for the user's device/browser combination (for advanced tracking)

        // Derived properties, if needed
        public string? LoginStatus { get; set; }

        private LoginAudit() { } // EF Core

        public static LoginAudit Create(string? userId, bool isSuccessful, string? ipAddress, string? userAgent)
        {
            return new LoginAudit
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IsSuccessful = isSuccessful,
                IPAddress = ipAddress,
                UserAgent = userAgent
            };
        }
    }
}
