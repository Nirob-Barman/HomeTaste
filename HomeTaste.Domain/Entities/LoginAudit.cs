namespace HomeTaste.Domain.Entities
{
    public class LoginAudit
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        public DateTime LoginTime { get; set; }

        public string? IPAddress { get; set; }

        public string? DeviceInfo { get; set; }

        public string? UserAgent { get; set; }

        public bool IsSuccessful { get; set; }

        public string? Location { get; set; }  // Optional: Geolocation of the user (could use IP-based geolocation)

        public string? ErrorMessage { get; set; }  // Optional: If login failed, this can store the error message (e.g., "Invalid credentials")

        public string? DeviceFingerprint { get; set; }  // Optional: A unique identifier for the user's device/browser combination (for advanced tracking)

        // Derived properties, if needed
        public string? LoginStatus { get; set; }
    }
}
